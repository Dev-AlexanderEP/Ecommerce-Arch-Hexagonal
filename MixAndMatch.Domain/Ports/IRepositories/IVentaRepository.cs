using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IVentaRepository : IGenericRepository<Venta>
{
    Task<bool> TieneDetalles(long ventaId);
    Task<bool> TienePagos(long ventaId);
    Task<bool> TieneEnvios(long ventaId);

    // Carga la venta junto con sus detalles (necesario para usar Venta.Total).
    Task<Venta?> GetByIdConDetalles(long ventaId);
}
