using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
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
}
