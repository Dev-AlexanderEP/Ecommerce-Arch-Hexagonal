using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IPrendaTallaRepository : IGenericRepository<PrendaTalla>
{
    Task<bool> ExisteCombinacion(long prendaId, long tallaId, long? exceptoId = null);
    Task<bool> TieneItemsCarrito(long prendaTallaId);
    Task<bool> TieneVentas(long prendaTallaId);
}
