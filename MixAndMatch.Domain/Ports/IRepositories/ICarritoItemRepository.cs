using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface ICarritoItemRepository : IGenericRepository<CarritoItem>
{
    Task<bool> ExisteEnCarrito(long carritoId, long prendaTallaId);
    Task<CarritoItem?> BuscarPorCarritoPrendaTalla(long carritoId, long prendaTallaId);
}
