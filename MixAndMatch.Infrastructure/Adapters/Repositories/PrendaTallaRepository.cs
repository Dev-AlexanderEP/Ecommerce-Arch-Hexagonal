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
}
