using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface ICarritoRepository : IGenericRepository<Carrito>
{
    Task<bool> TieneCarritoActivo(long usuarioId);
    Task<bool> TieneItems(long carritoId);

    // Jobs de mantenimiento
    Task<int> MarcarAbandonadosAsync(int diasInactividad);
    Task<decimal> GetTotalCarritoActivo(long usuarioId);

    // Migration endpoints
    Task<List<Carrito>> BuscarAbiertosPorUsuarioId(long usuarioId);
    Task<int> ContarItemsDistintos(long carritoId);
}
