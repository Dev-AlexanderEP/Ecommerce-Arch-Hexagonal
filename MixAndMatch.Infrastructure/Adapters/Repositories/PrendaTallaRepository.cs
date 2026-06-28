using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class PrendaTallaRepository(MixAndMatchDbContext context)
    : GenericRepository<PrendaTalla>(context), IPrendaTallaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteCombinacion(long prendaId, long tallaId, long? exceptoId = null) =>
        _context.Set<PrendaTalla>()
            .AnyAsync(pt => pt.PrendaId == prendaId && pt.TallaId == tallaId && (exceptoId == null || pt.Id != exceptoId));

    public Task<bool> TieneItemsCarrito(long prendaTallaId) =>
        _context.Set<CarritoItem>().AnyAsync(ci => ci.PrendaTallaId == prendaTallaId);

    public Task<bool> TieneVentas(long prendaTallaId) =>
        _context.Set<VentasDetalle>().AnyAsync(vd => vd.PrendaTallaId == prendaTallaId);

    public async Task<int> RestarUnoStock(long prendaId, long tallaId)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<PrendaTalla>()
            .Where(pt => pt.PrendaId == prendaId && pt.TallaId == tallaId && pt.Stock >= 1)
            .ExecuteUpdateAsync(s => s
                .SetProperty(pt => pt.Stock, pt => pt.Stock - 1)
                .SetProperty(pt => pt.UpdatedAt, now));
    }

    public async Task<int> SumarUnoStock(long prendaId, long tallaId)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<PrendaTalla>()
            .Where(pt => pt.PrendaId == prendaId && pt.TallaId == tallaId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(pt => pt.Stock, pt => pt.Stock + 1)
                .SetProperty(pt => pt.UpdatedAt, now));
    }

    public async Task<int> SumarStock(long prendaId, long tallaId, int cantidad)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<PrendaTalla>()
            .Where(pt => pt.PrendaId == prendaId && pt.TallaId == tallaId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(pt => pt.Stock, pt => pt.Stock + cantidad)
                .SetProperty(pt => pt.UpdatedAt, now));
    }

    public Task<PrendaTalla?> BuscarPorPrendaYTalla(long prendaId, long tallaId) =>
        _context.Set<PrendaTalla>()
            .FirstOrDefaultAsync(pt => pt.PrendaId == prendaId && pt.TallaId == tallaId);
}
