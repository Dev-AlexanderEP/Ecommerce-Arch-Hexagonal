using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Chat;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Tools;

public class BuscarPrendasTool(MixAndMatchDbContext context)
{
    public static string Nombre => "buscar_prendas";

    public static string Descripcion =>
        "Busca prendas de ropa en el catálogo de MixAndMatch. " +
        "Úsala cuando el usuario quiera ver productos, explorar el catálogo o buscar por tipo, género o precio. " +
        "Devuelve hasta 5 prendas con imagen, precio, género y categoría.";

    public static object ParametrosSchema => new
    {
        type = "object",
        properties = new
        {
            nombre     = new { type = "string", description = "Nombre o palabra clave de la prenda (opcional)" },
            genero     = new { type = "string", description = "Género: Mujer, Hombre, Niño, Niña (opcional)" },
            categoria  = new { type = "string", description = "Categoría: Abrigos, Blazers, Polos, etc. (opcional)" },
            precio_min = new { type = "number", description = "Precio mínimo en soles (opcional)" },
            precio_max = new { type = "number", description = "Precio máximo en soles (opcional)" },
        },
        required = Array.Empty<string>(),
    };

    public async Task<ChatToolResultado> EjecutarAsync(JsonElement args)
    {
        var nombre    = args.TryGetProperty("nombre",    out var n) ? n.GetString() : null;
        var genero    = args.TryGetProperty("genero",    out var g) ? g.GetString() : null;
        var categoria = args.TryGetProperty("categoria", out var c) ? c.GetString() : null;

        decimal? precioMin = args.TryGetProperty("precio_min", out var pMin) && pMin.ValueKind == JsonValueKind.Number
            ? pMin.GetDecimal() : null;
        decimal? precioMax = args.TryGetProperty("precio_max", out var pMax) && pMax.ValueKind == JsonValueKind.Number
            ? pMax.GetDecimal() : null;

        var query = context.Set<Prenda>()
            .Where(p => p.Activo)
            .Include(p => p.Genero)
            .Include(p => p.Categoria)
            .Include(p => p.PrendaImagens)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(p => EF.Functions.ILike(p.Nombre, $"%{nombre}%"));

        if (!string.IsNullOrWhiteSpace(genero))
            query = query.Where(p => EF.Functions.ILike(p.Genero.NomGenero, $"%{genero}%"));

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(p => EF.Functions.ILike(p.Categoria.NomCategoria, $"%{categoria}%"));

        if (precioMin.HasValue)
            query = query.Where(p => p.Precio >= precioMin.Value);

        if (precioMax.HasValue)
            query = query.Where(p => p.Precio <= precioMax.Value);

        var prendas = await query.Take(5).ToListAsync();

        if (prendas.Count == 0)
            return new ChatToolResultado { ResumenParaIA = "No se encontraron prendas con esos filtros." };

        var productos = prendas.Select(p => new ChatProductoDto
        {
            Id        = p.Id,
            Nombre    = p.Nombre,
            Precio    = p.Precio,
            Genero    = p.Genero.NomGenero,
            Categoria = p.Categoria.NomCategoria,
            ImagenUrl = p.PrendaImagens
                            .Where(i => i.Tipo == TipoImagen.PRINCIPAL)
                            .OrderBy(i => i.Orden)
                            .Select(i => i.Url)
                            .FirstOrDefault()
                        ?? p.PrendaImagens
                            .OrderBy(i => i.Orden)
                            .Select(i => i.Url)
                            .FirstOrDefault(),
        }).ToList();

        var resumen = string.Join("\n", productos.Select((p, i) =>
            $"{i + 1}. {p.Nombre} | {p.Categoria} | {p.Genero} | S/. {p.Precio:0.00}"));

        return new ChatToolResultado
        {
            ResumenParaIA = $"Encontré {productos.Count} prenda(s):\n{resumen}",
            Productos     = productos,
        };
    }
}
