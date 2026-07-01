using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MixAndMatch.Domain.DTOs.Chat;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Adapters.Tools;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class DeepSeekService(
    HttpClient httpClient,
    IOptions<DeepSeekSettings> options,
    BuscarPrendasTool buscarPrendas,
    ConocimientoBaseTool conocimientoBase) : IChatIAService
{
    private readonly DeepSeekSettings _cfg = options.Value;
    private const int MAX_ITER = 5;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    // Descriptor interno — une nombre, schema y función de ejecución
    private record ToolEntry(
        string Nombre,
        string Descripcion,
        object Schema,
        Func<JsonElement, Task<ChatToolResultado>> Ejecutar);

    private List<ToolEntry> BuildTools() =>
    [
        new(BuscarPrendasTool.Nombre,     BuscarPrendasTool.Descripcion,     BuscarPrendasTool.ParametrosSchema,     buscarPrendas.EjecutarAsync),
        new(ConocimientoBaseTool.Nombre,  ConocimientoBaseTool.Descripcion,  ConocimientoBaseTool.ParametrosSchema,  conocimientoBase.EjecutarAsync),
    ];

    public async Task<ChatRespuestaDto> Preguntar(string mensaje)
    {
        var systemPrompt = ConstruirSystemPrompt();
        var toolList     = BuildTools();

        var toolsSchema = toolList.Select(t => (object)new
        {
            type     = "function",
            function = new
            {
                name        = t.Nombre,
                description = t.Descripcion,
                parameters  = t.Schema,
            },
        }).ToList();

        var messages = new List<object>
        {
            new { role = "system", content = systemPrompt },
            new { role = "user",   content = mensaje },
        };

        var productosAcumulados = new List<ChatProductoDto>();

        for (int iter = 0; iter < MAX_ITER; iter++)
        {
            var body = new
            {
                model       = _cfg.Model,
                messages,
                tools       = toolsSchema.Count > 0 ? toolsSchema : null,
                tool_choice = toolsSchema.Count > 0 ? "auto" : null,
                max_tokens  = 512,
                temperature = 0.7,
            };

            var httpReq = new HttpRequestMessage(HttpMethod.Post, $"{_cfg.BaseUrl}/chat/completions")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(body, _json),
                    Encoding.UTF8,
                    "application/json")
            };
            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cfg.ApiKey);

            var httpResp = await httpClient.SendAsync(httpReq);
            httpResp.EnsureSuccessStatusCode();

            var responseBody = await httpResp.Content.ReadAsStringAsync();
            string finishReason;
            string assistantRaw;

            using (var doc = JsonDocument.Parse(responseBody))
            {
                var choice   = doc.RootElement.GetProperty("choices")[0];
                finishReason = choice.GetProperty("finish_reason").GetString()!;
                assistantRaw = choice.GetProperty("message").GetRawText();
            }

            if (finishReason == "stop")
            {
                using var msgDoc = JsonDocument.Parse(assistantRaw);
                var texto = msgDoc.RootElement.GetProperty("content").GetString()
                            ?? "Lo siento, no pude generar una respuesta.";

                return new ChatRespuestaDto
                {
                    Texto     = texto,
                    Productos = productosAcumulados.Count > 0 ? productosAcumulados : null,
                };
            }

            if (finishReason == "tool_calls")
            {
                messages.Add(JsonSerializer.Deserialize<JsonElement>(assistantRaw));

                using var msgDoc  = JsonDocument.Parse(assistantRaw);
                var toolCalls = msgDoc.RootElement.GetProperty("tool_calls");

                foreach (var toolCall in toolCalls.EnumerateArray())
                {
                    var toolCallId = toolCall.GetProperty("id").GetString()!;
                    var toolName   = toolCall.GetProperty("function").GetProperty("name").GetString()!;
                    var toolArgs   = toolCall.GetProperty("function").GetProperty("arguments").GetString()!;

                    var entry = toolList.FirstOrDefault(t => t.Nombre == toolName);
                    string resultContent;

                    if (entry is null)
                    {
                        resultContent = $"Tool '{toolName}' no encontrada.";
                    }
                    else
                    {
                        using var argsDoc = JsonDocument.Parse(toolArgs);
                        var resultado = await entry.Ejecutar(argsDoc.RootElement);

                        if (resultado.Productos is not null)
                            productosAcumulados.AddRange(resultado.Productos);

                        resultContent = resultado.ResumenParaIA;
                    }

                    messages.Add(new
                    {
                        role         = "tool",
                        tool_call_id = toolCallId,
                        content      = resultContent,
                    });
                }

                continue;
            }

            break;
        }

        return new ChatRespuestaDto
        {
            Texto     = "Procesé tu consulta pero no pude generar una respuesta final.",
            Productos = productosAcumulados.Count > 0 ? productosAcumulados : null,
        };
    }

    private static string ConstruirSystemPrompt() => """
        Eres Luna, la asistente virtual de MixAndMatch — una tienda de moda online peruana
        que ofrece prendas de calidad para toda la familia: hombres, mujeres, niños y niñas.
        La propuesta de valor de MixAndMatch es permitir a los clientes combinar prendas
        fácilmente para crear outfits completos con estilo propio.

        TU PERSONALIDAD:
        - Amable, cercana y entusiasta de la moda, pero siempre concisa.
        - Usas un tono conversacional, como una asesora de moda de confianza.
        - Respondes siempre en español.
        - Tus respuestas no superan 3-4 oraciones salvo que el usuario pida más detalle.

        HERRAMIENTAS QUE TIENES:
        - conocimiento_base: úsala cuando el usuario pregunte qué hay disponible, qué
          categorías o géneros maneja la tienda, o cuál es el rango de precios general.
          Llámala antes de responder preguntas generales sobre el catálogo.
        - buscar_prendas: úsala cuando el usuario quiera ver productos concretos, pida
          sugerencias de prendas, o filtre por tipo, género o precio. Puedes combinar
          filtros (ej. género + precio_max). Devuelve hasta 5 prendas con imagen.

        FORMATO:
        - Usa markdown en tus respuestas: **negrita** para nombres de prendas o precios,
          listas numeradas o con guiones para varias opciones, y texto plano para frases cortas.
        - No uses encabezados (#, ##) — el chat no los necesita.

        REGLAS DE COMPORTAMIENTO:
        - Nunca inventes precios exactos, tallas disponibles ni stock. Usa las tools.
        - Si el usuario pregunta algo ajeno a la tienda (política, recetas, etc.), responde
          amablemente: "Solo puedo ayudarte con todo lo relacionado a MixAndMatch. ¿En qué
          te puedo orientar sobre nuestras prendas?"
        - Cuando sugiereas outfits o combinaciones, usa el nombre real de categorías
          (obtenidas con conocimiento_base) para que el usuario sepa qué buscar.
        - Si la búsqueda no arroja resultados, sugiere ampliar los filtros o usar
          conocimiento_base para ver qué otras opciones hay.
        """;

}
