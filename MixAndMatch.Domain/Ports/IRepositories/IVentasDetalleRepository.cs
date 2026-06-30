using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IVentasDetalleRepository : IGenericRepository<VentasDetalle>
{
    Task<bool> ExisteEnVenta(long ventaId, long prendaTallaId, long? exceptoId = null);
    Task DeleteByVentaId(long ventaId);
}
