using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.DTOs.Chat;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Tools;

public class ConocimientoBaseTool(MixAndMatchDbContext context)
{
    public static string Nombre => "conocimiento_base";

    public static string Descripcion =>
        "Devuelve información general del catálogo de MixAndMatch: géneros disponibles, " +
        "categorías de ropa, rango de precios y total de prendas activas. " +
        "Úsala cuando el usuario pregunte qué hay disponible, qué categorías existen, " +
        "cuánto cuestan las prendas en general o qué géneros maneja la tienda.";

    public static object ParametrosSchema => new
    {
        type = "object",
        properties = new { },
        required = Array.Empty<string>(),
    };

    public async Task<ChatToolResultado> EjecutarAsync(JsonElement _)
    {
        var generos = await context.Set<Genero>()
            .Select(g => g.NomGenero)
            .ToListAsync();

        var categorias = await context.Set<Categoria>()
            .Select(c => c.NomCategoria)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        var stats = await context.Set<Prenda>()
            .Where(p => p.Activo)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Min   = g.Min(p => p.Precio),
                Max   = g.Max(p => p.Precio),
            })
            .FirstOrDefaultAsync();

        var resumen = $"""
            CATÁLOGO MIXANDMATCH:
            - Géneros disponibles: {string.Join(", ", generos)}
            - Categorías disponibles ({categorias.Count}): {string.Join(", ", categorias)}
            - Rango de precios: S/. {stats?.Min:0.00} – S/. {stats?.Max:0.00}
            - Total de prendas activas: {stats?.Total ?? 0}
            """;

        return new ChatToolResultado { ResumenParaIA = resumen };
    }
}
