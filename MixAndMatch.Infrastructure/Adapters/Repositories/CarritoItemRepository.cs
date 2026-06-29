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

    public Task<CarritoItem?> BuscarPorCarritoPrendaTalla(long carritoId, long prendaTallaId) =>
        _context.Set<CarritoItem>()
            .FirstOrDefaultAsync(i => i.CarritoId == carritoId && i.PrendaTallaId == prendaTallaId);

    public Task<List<CarritoItem>> GetByCarritoId(long carritoId) =>
        _context.Set<CarritoItem>()
            .Where(i => i.CarritoId == carritoId)
            .ToListAsync();

    public Task<List<CarritoItem>> GetDetalladosByCarritoId(long carritoId) =>
        _context.Set<CarritoItem>()
            .Where(i => i.CarritoId == carritoId)
            .Include(i => i.PrendaTalla)
                .ThenInclude(pt => pt.Prenda)
                    .ThenInclude(p => p.Marca)
            .Include(i => i.PrendaTalla)
                .ThenInclude(pt => pt.Prenda)
                    .ThenInclude(p => p.PrendaImagens)
            .Include(i => i.PrendaTalla)
                .ThenInclude(pt => pt.Talla)
            .ToListAsync();
}
