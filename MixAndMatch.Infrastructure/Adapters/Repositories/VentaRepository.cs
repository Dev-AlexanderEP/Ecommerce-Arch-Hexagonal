using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class VentaRepository(MixAndMatchDbContext context)
    : GenericRepository<Venta>(context), IVentaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> TieneDetalles(long ventaId) =>
        _context.Set<VentasDetalle>().AnyAsync(d => d.VentaId == ventaId);

    public Task<bool> TienePagos(long ventaId) =>
        _context.Set<Pago>().AnyAsync(p => p.VentaId == ventaId);

    public Task<bool> TieneEnvios(long ventaId) =>
        _context.Set<Envio>().AnyAsync(e => e.VentaId == ventaId);

    public Task<Venta?> GetByIdConDetalles(long ventaId) =>
        _context.Set<Venta>()
            .Include(v => v.VentasDetalles)
            .FirstOrDefaultAsync(v => v.Id == ventaId);

    public async Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedConFiltro(
        string? nombreUsuario, int page, int pageSize)
    {
        var query = _context.Set<Venta>()
            .Include(v => v.Usuario)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombreUsuario))
            query = query.Where(v => EF.Functions.ILike(v.Usuario.NombreUsuario, $"%{nombreUsuario}%"));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(v => v.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public Task<long?> GetSegundaPendienteId(long usuarioId) =>
        _context.Set<Venta>()
            .Where(v => v.UsuarioId == usuarioId && v.Estado == EstadoVenta.PENDIENTE)
            .OrderBy(v => v.FechaCreacion)
            .Skip(1)
            .Select(v => (long?)v.Id)
            .FirstOrDefaultAsync();

    public Task<int> GetTotalRealizadas() =>
        _context.Set<Venta>()
            .CountAsync(v => v.Estado == EstadoVenta.PAGADO
                          || v.Estado == EstadoVenta.ENVIADO
                          || v.Estado == EstadoVenta.ENTREGADO);

    public async Task<List<VentasPorPeriodoDto>> GetVentasPorPeriodo(string agrupacion)
    {
        // Nota: EF/Npgsql no traduce ToString() ni string interpolation a SQL.
        // Se agrupa por componentes numéricos en la BD y se formatea el string en memoria.
        var query = _context.Set<Venta>()
            .AsNoTracking()
            .Where(v => v.Estado == EstadoVenta.PAGADO
                     || v.Estado == EstadoVenta.ENVIADO
                     || v.Estado == EstadoVenta.ENTREGADO);

        return agrupacion.ToLowerInvariant() switch
        {
            "semanal" => (await query
                .GroupBy(v => new { v.FechaCreacion.Year, Semana = (v.FechaCreacion.DayOfYear - 1) / 7 + 1 })
                .Select(g => new { g.Key.Year, g.Key.Semana, CantidadVentas = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Semana)
                .ToListAsync())
                .Select(x => new VentasPorPeriodoDto
                {
                    Periodo        = $"{x.Year}-S{x.Semana:D2}",
                    CantidadVentas = x.CantidadVentas,
                }).ToList(),

            "mensual" => (await query
                .GroupBy(v => new { v.FechaCreacion.Year, v.FechaCreacion.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, CantidadVentas = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync())
                .Select(x => new VentasPorPeriodoDto
                {
                    Periodo        = $"{x.Year}-{x.Month:D2}",
                    CantidadVentas = x.CantidadVentas,
                }).ToList(),

            "anual" => (await query
                .GroupBy(v => v.FechaCreacion.Year)
                .Select(g => new { Year = g.Key, CantidadVentas = g.Count() })
                .OrderBy(x => x.Year)
                .ToListAsync())
                .Select(x => new VentasPorPeriodoDto
                {
                    Periodo        = x.Year.ToString(),
                    CantidadVentas = x.CantidadVentas,
                }).ToList(),

            // diario (default): últimos 30 días agrupados por fecha
            _ => (await query
                .Where(v => v.FechaCreacion >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(v => new { v.FechaCreacion.Year, v.FechaCreacion.Month, v.FechaCreacion.Day })
                .Select(g => new { g.Key.Year, g.Key.Month, g.Key.Day, CantidadVentas = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync())
                .Select(x => new VentasPorPeriodoDto
                {
                    Periodo        = $"{x.Year}-{x.Month:D2}-{x.Day:D2}",
                    CantidadVentas = x.CantidadVentas,
                }).ToList(),
        };
    }

    public Task<List<VentasPorGeneroDto>> GetVentasPorGenero() =>
        _context.Set<VentasDetalle>()
            .AsNoTracking()
            .Include(d => d.Venta)
            .Include(d => d.PrendaTalla)
                .ThenInclude(pt => pt.Prenda)
                    .ThenInclude(p => p.Genero)
            .Where(d => d.Venta.Estado == EstadoVenta.PAGADO
                     || d.Venta.Estado == EstadoVenta.ENVIADO
                     || d.Venta.Estado == EstadoVenta.ENTREGADO)
            .GroupBy(d => d.PrendaTalla.Prenda.Genero.NomGenero)
            .Select(g => new VentasPorGeneroDto
            {
                Genero         = g.Key,
                CantidadVentas = g.Select(d => d.VentaId).Distinct().Count(),
            })
            .OrderByDescending(x => x.CantidadVentas)
            .ToListAsync();
}
