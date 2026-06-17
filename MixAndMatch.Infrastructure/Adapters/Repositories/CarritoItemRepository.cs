using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class CarritoItemRepository(MixAndMatchDbContext context)
    : GenericRepository<CarritoItem>(context), ICarritoItemRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteEnCarrito(long carritoId, long prendaTallaId) =>
        _context.Set<CarritoItem>()
            .AnyAsync(i => i.CarritoId == carritoId && i.PrendaTallaId == prendaTallaId);
}
