using Microsoft.EntityFrameworkCore;
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
}
