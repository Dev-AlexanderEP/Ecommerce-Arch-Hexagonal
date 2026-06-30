using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class VentasDetalleRepository(MixAndMatchDbContext context)
    : GenericRepository<VentasDetalle>(context), IVentasDetalleRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteEnVenta(long ventaId, long prendaTallaId, long? exceptoId = null) =>
        _context.Set<VentasDetalle>()
            .AnyAsync(d => d.VentaId == ventaId && d.PrendaTallaId == prendaTallaId && (exceptoId == null || d.Id != exceptoId));

    public async Task DeleteByVentaId(long ventaId)
    {
        var detalles = await _context.Set<VentasDetalle>()
            .Where(d => d.VentaId == ventaId)
            .ToListAsync();
        _context.Set<VentasDetalle>().RemoveRange(detalles);
    }
}
