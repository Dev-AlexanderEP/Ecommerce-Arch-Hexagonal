using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class CarritoRepository(MixAndMatchDbContext context)
    : GenericRepository<Carrito>(context), ICarritoRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> TieneCarritoActivo(long usuarioId) =>
        _context.Set<Carrito>()
            .AnyAsync(c => c.UsuarioId == usuarioId && c.Estado == EstadoCarrito.ACTIVO);

    public Task<bool> TieneItems(long carritoId) =>
        _context.Set<CarritoItem>()
            .AnyAsync(i => i.CarritoId == carritoId);

    public async Task<int> MarcarAbandonadosAsync(int diasInactividad)
    {
        var umbral = DateTime.UtcNow.AddDays(-diasInactividad);
        return await _context.Set<Carrito>()
            .Where(c => c.Estado == EstadoCarrito.ACTIVO &&
                        (c.UpdatedAt != null && c.UpdatedAt < umbral ||
                         c.UpdatedAt == null && c.FechaCreacion != null && c.FechaCreacion < umbral))
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Estado, EstadoCarrito.ABANDONADO)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));
    }

    public Task<List<Carrito>> BuscarAbiertosPorUsuarioId(long usuarioId) =>
        _context.Set<Carrito>()
            .Where(c => c.UsuarioId == usuarioId && c.Estado == EstadoCarrito.ACTIVO)
            .ToListAsync();
            
     public Task<decimal> GetTotalCarritoActivo(long usuarioId) =>
        (from c in _context.Set<Carrito>()
         join i in _context.Set<CarritoItem>() on c.Id equals i.CarritoId
         where c.UsuarioId == usuarioId && c.Estado == EstadoCarrito.ACTIVO
         select i.PrecioUnitario * i.Cantidad)
        .SumAsync();

    public Task<int> ContarItemsDistintos(long carritoId) =>
        _context.Set<CarritoItem>()
            .CountAsync(i => i.CarritoId == carritoId);
}
