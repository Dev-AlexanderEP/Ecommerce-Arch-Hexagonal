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
}
