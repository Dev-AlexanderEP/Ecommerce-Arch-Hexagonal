using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.DTOs;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class PrendaRepository(MixAndMatchDbContext context)
    : GenericRepository<Prenda>(context), IPrendaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> TieneDescuentos(long prendaId) =>
        _context.Set<DescuentoPrenda>().AnyAsync(d => d.PrendaId == prendaId);

    public Task<bool> TieneImagenes(long prendaId) =>
        _context.Set<PrendaImagen>().AnyAsync(i => i.PrendaId == prendaId);

    public Task<bool> TieneTallas(long prendaId) =>
        _context.Set<PrendaTalla>().AnyAsync(t => t.PrendaId == prendaId);

    public Task<bool> TieneResenias(long prendaId) =>
        _context.Set<Resenia>().AnyAsync(r => r.PrendaId == prendaId);

    public async Task<List<string>> BuscarTallasPorCategoria(string categoria)
    {
        var term = $"%{categoria}%";
        return await _context.Set<Prenda>()
            .Where(p => p.Activo && p.Categoria.NomCategoria.ToLower().Contains(categoria.ToLower()))
            .SelectMany(p => p.PrendaTallas)
            .Select(pt => pt.Talla.NomTalla)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    public async Task<List<string>> BuscarTallasPorGenero(string genero)
    {
        return await _context.Set<Prenda>()
            .Where(p => p.Activo && p.Genero.NomGenero.ToLower().Contains(genero.ToLower()))
            .SelectMany(p => p.PrendaTallas)
            .Select(pt => pt.Talla.NomTalla)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    public async Task<List<string>> BuscarMarcasPorCategoria(string categoria)
    {
        return await _context.Set<Prenda>()
            .Where(p => p.Categoria.NomCategoria.ToLower().Contains(categoria.ToLower()))
            .Select(p => p.Marca.NomMarca)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }

    public async Task<List<string>> BuscarMarcasPorGenero(string genero)
    {
        return await _context.Set<Prenda>()
            .Where(p => p.Genero.NomGenero.ToLower().Contains(genero.ToLower()))
            .Select(p => p.Marca.NomMarca)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }

    public async Task<PrendaPrecioStatsDto> BuscarEstadisticasPreciosPorCategoria(string categoria)
    {
        var query = _context.Set<Prenda>()
            .Where(p => p.Categoria.NomCategoria.ToLower().Contains(categoria.ToLower()));

        var min = await query.MinAsync(p => (decimal?)p.Precio);
        if (min is null) return new PrendaPrecioStatsDto();

        var avg = await query.AverageAsync(p => (decimal?)p.Precio);
        var max = await query.MaxAsync(p => (decimal?)p.Precio);

        return new PrendaPrecioStatsDto
        {
            Minimo = min,
            Promedio = avg.HasValue ? Math.Round(avg.Value, 2) : null,
            Maximo = max
        };
    }

    public async Task<PrendaPrecioStatsDto> BuscarEstadisticasPreciosPorGenero(string genero)
    {
        var query = _context.Set<Prenda>()
            .Where(p => p.Genero.NomGenero.ToLower().Contains(genero.ToLower()));

        var min = await query.MinAsync(p => (decimal?)p.Precio);
        if (min is null) return new PrendaPrecioStatsDto();

        var avg = await query.AverageAsync(p => (decimal?)p.Precio);
        var max = await query.MaxAsync(p => (decimal?)p.Precio);

        return new PrendaPrecioStatsDto
        {
            Minimo = min,
            Promedio = avg.HasValue ? Math.Round(avg.Value, 2) : null,
            Maximo = max
        };
    }

    public async Task<List<decimal>> BuscarDescuentosPorCategoria(string categoria)
    {
        FormattableString sql = $@"
            SELECT DISTINCT COALESCE(dp.porcentaje, dc.porcentaje) AS value
            FROM prenda p
            JOIN categoria c ON c.id = p.categoria_id
            LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id
                AND dp.activo = true
                AND dp.fecha_inicio <= CURRENT_DATE
                AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE)
            LEFT JOIN descuento_categoria dc ON dc.categoria_id = c.id
                AND dc.activo = true
                AND dc.fecha_inicio <= CURRENT_DATE
                AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE)
            WHERE p.activo = true
            AND LOWER(c.nom_categoria) LIKE LOWER(CONCAT('%', {categoria}, '%'))
            AND (dp.porcentaje IS NOT NULL OR dc.porcentaje IS NOT NULL)";

        return await _context.Database.SqlQuery<decimal>(sql).ToListAsync();
    }

    public async Task<List<decimal>> BuscarDescuentosPorGenero(string genero)
    {
        FormattableString sql = $@"
            SELECT DISTINCT COALESCE(dp.porcentaje, dc.porcentaje) AS value
            FROM prenda p
            JOIN genero g ON g.id = p.genero_id
            LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id
                AND dp.activo = true
                AND dp.fecha_inicio <= CURRENT_DATE
                AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE)
            LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id
                AND dc.activo = true
                AND dc.fecha_inicio <= CURRENT_DATE
                AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE)
            WHERE p.activo = true
            AND LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', {genero}, '%'))
            AND (dp.porcentaje IS NOT NULL OR dc.porcentaje IS NOT NULL)";

        return await _context.Database.SqlQuery<decimal>(sql).ToListAsync();
    }

    public async Task<List<string>> BuscarCategoriasPorGenero(string genero)
    {
        return await _context.Set<Prenda>()
            .Where(p => p.Genero.NomGenero.ToLower().Contains(genero.ToLower()))
            .Select(p => p.Categoria.NomCategoria)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<List<PrendaConDescuentoResponseDto>> BuscarPrendasConDescuento(string? nombre, string? categoria, string? genero)
    {
        FormattableString sql = $@"
            SELECT
                p.id AS ""Id"",
                p.nombre AS ""Nombre"",
                p.precio AS ""Precio"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'PRINCIPAL' LIMIT 1) AS ""ImagenPrincipal"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'SECUNDARIA' LIMIT 1) AS ""ImagenHover"",
                m.nom_marca AS ""Marca"",
                COALESCE(dp.porcentaje, dc.porcentaje, 0) AS ""DescuentoAplicado"",
                CASE
                    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda'
                    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria'
                    ELSE 'sin_descuento'
                END AS ""TipoDescuento""
            FROM prenda p
            JOIN categoria c ON c.id = p.categoria_id
            JOIN marca m ON m.id = p.marca_id
            JOIN genero g ON g.id = p.genero_id
            LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id
                AND dp.activo = true
                AND dp.fecha_inicio <= CURRENT_DATE
                AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE)
            LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id
                AND dc.activo = true
                AND dc.fecha_inicio <= CURRENT_DATE
                AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE)
            WHERE p.activo = true
            AND ({nombre} IS NULL OR LOWER(p.nombre) LIKE LOWER(CONCAT('%', {nombre}, '%')))
            AND ({categoria} IS NULL OR LOWER(c.nom_categoria) LIKE LOWER(CONCAT('%', {categoria}, '%')))
            AND ({genero} IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', {genero}, '%')))";

        return await _context.Database.SqlQuery<PrendaConDescuentoResponseDto>(sql).ToListAsync();
    }

    public async Task<List<PrendaConDescuentoResponseDto>> BuscarDescuentosAplicados(string? categoria, string? genero)
    {
        FormattableString sql = $@"
            SELECT
                p.id AS ""Id"",
                p.nombre AS ""Nombre"",
                p.precio AS ""Precio"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'PRINCIPAL' LIMIT 1) AS ""ImagenPrincipal"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'SECUNDARIA' LIMIT 1) AS ""ImagenHover"",
                m.nom_marca AS ""Marca"",
                COALESCE(dp.porcentaje, dc.porcentaje, 0) AS ""DescuentoAplicado"",
                CASE
                    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda'
                    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria'
                    ELSE 'sin_descuento'
                END AS ""TipoDescuento""
            FROM prenda p
            JOIN categoria c ON c.id = p.categoria_id
            JOIN marca m ON m.id = p.marca_id
            JOIN genero g ON g.id = p.genero_id
            LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id
                AND dp.activo = true
                AND dp.fecha_inicio <= CURRENT_DATE
                AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE)
            LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id
                AND dc.activo = true
                AND dc.fecha_inicio <= CURRENT_DATE
                AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE)
            WHERE p.activo = true
            AND ({categoria} IS NULL OR LOWER(c.nom_categoria) LIKE LOWER(CONCAT('%', {categoria}, '%')))
            AND ({genero} IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', {genero}, '%')))";

        return await _context.Database.SqlQuery<PrendaConDescuentoResponseDto>(sql).ToListAsync();
    }

    public async Task<List<PrendaConDescuentoTodoResponseDto>> BuscarDescuentosAplicadosAleatorio(string genero)
    {
        FormattableString sql = $@"
            SELECT
                p.id AS ""Id"",
                p.nombre AS ""Nombre"",
                p.precio AS ""Precio"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'PRINCIPAL' LIMIT 1) AS ""ImagenPrincipal"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'SECUNDARIA' LIMIT 1) AS ""ImagenHover"",
                m.nom_marca AS ""Marca"",
                COALESCE(dp.porcentaje, dc.porcentaje, 0) AS ""DescuentoAplicado"",
                CASE
                    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda'
                    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria'
                    ELSE 'sin_descuento'
                END AS ""TipoDescuento"",
                c.nom_categoria AS ""Categoria""
            FROM prenda p
            JOIN categoria c ON c.id = p.categoria_id
            JOIN marca m ON m.id = p.marca_id
            JOIN genero g ON g.id = p.genero_id
            LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id
                AND dp.activo = true
                AND dp.fecha_inicio <= CURRENT_DATE
                AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE)
            LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id
                AND dc.activo = true
                AND dc.fecha_inicio <= CURRENT_DATE
                AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE)
            WHERE p.activo = true
            AND ({genero} IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', {genero}, '%')))
            ORDER BY RANDOM()";

        return await _context.Database.SqlQuery<PrendaConDescuentoTodoResponseDto>(sql).ToListAsync();
    }

    public async Task<List<PrendaConDescuentoResponseDto>> FiltrarDinamico(string? talla, string? categoria, string? marca, string? genero, double? precioMin, double? precioMax, double? descMin, double? descMax)
    {
        FormattableString sql = $@"
            SELECT
                p.id AS ""Id"",
                p.nombre AS ""Nombre"",
                p.precio AS ""Precio"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'PRINCIPAL' LIMIT 1) AS ""ImagenPrincipal"",
                (SELECT pi.url FROM prenda_imagen pi WHERE pi.prenda_id = p.id AND pi.tipo = 'SECUNDARIA' LIMIT 1) AS ""ImagenHover"",
                m.nom_marca AS ""Marca"",
                COALESCE(dp.porcentaje, dc.porcentaje, 0) AS ""DescuentoAplicado"",
                CASE
                    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda'
                    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria'
                    ELSE 'sin_descuento'
                END AS ""TipoDescuento""
            FROM prenda p
            JOIN marca m ON p.marca_id = m.id
            JOIN categoria c ON p.categoria_id = c.id
            JOIN genero g ON p.genero_id = g.id
            LEFT JOIN descuento_prenda dp ON p.id = dp.prenda_id
                AND dp.activo = true
                AND dp.fecha_inicio <= CURRENT_DATE
                AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE)
            LEFT JOIN descuento_categoria dc ON p.categoria_id = dc.categoria_id
                AND dc.activo = true
                AND dc.fecha_inicio <= CURRENT_DATE
                AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE)
            WHERE p.activo = true
            AND (CAST({talla} AS text) IS NULL OR EXISTS (
                SELECT 1 FROM prenda_talla pt
                JOIN talla t ON pt.talla_id = t.id
                WHERE pt.prenda_id = p.id AND t.nom_talla = {talla}
            ))
            AND (CAST({categoria} AS text) IS NULL OR c.nom_categoria = {categoria})
            AND (CAST({marca} AS text) IS NULL OR m.nom_marca = {marca})
            AND (CAST({genero} AS text) IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', {genero}, '%')))
            AND ((CAST({precioMin} AS double precision) IS NULL OR CAST({precioMax} AS double precision) IS NULL) OR (p.precio BETWEEN {precioMin} AND {precioMax}))
            AND ((CAST({descMin} AS double precision) IS NULL OR CAST({descMax} AS double precision) IS NULL) OR (COALESCE(dp.porcentaje, dc.porcentaje, 0) BETWEEN {descMin} AND {descMax}))
            ORDER BY p.nombre";

        return await _context.Database.SqlQuery<PrendaConDescuentoResponseDto>(sql).ToListAsync();
    }

    public Task<Prenda?> GetDetalladoById(long id) =>
        _context.Set<Prenda>()
            .Where(p => p.Id == id)
            .Include(p => p.Marca)
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Include(p => p.PrendaImagens)
            .Include(p => p.PrendaTallas)
                .ThenInclude(pt => pt.Talla)
            .FirstOrDefaultAsync(p => p.Id == id);
}
