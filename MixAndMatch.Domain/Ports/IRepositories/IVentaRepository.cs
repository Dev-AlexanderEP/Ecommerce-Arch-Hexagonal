using MixAndMatch.Domain.DTOs.Ventas;
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IVentaRepository : IGenericRepository<Venta>
{
    Task<bool> TieneDetalles(long ventaId);
    Task<bool> TienePagos(long ventaId);
    Task<bool> TieneEnvios(long ventaId);

    // Carga la venta junto con sus detalles (necesario para usar Venta.Total).
    Task<Venta?> GetByIdConDetalles(long ventaId);

    Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedConFiltro(string? nombreUsuario, int page, int pageSize);

    // Migration: buscar la segunda venta pendiente de un usuario
    Task<long?> GetSegundaPendienteId(long usuarioId);

    Task<int> GetTotalRealizadas();
    Task<List<VentasPorPeriodoDto>> GetVentasPorPeriodo(string agrupacion);
    Task<List<VentasPorGeneroDto>> GetVentasPorGenero();
}
