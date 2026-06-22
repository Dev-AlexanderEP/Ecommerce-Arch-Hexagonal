using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IEnvioRepository : IGenericRepository<Envio>
{
    Task<bool> ExisteParaVenta(long ventaId, long? exceptoId = null);
    Task<Envio?> FindByTrackingNumber(string trackingNumber);
    Task<IEnumerable<Envio>> GetByUsuarioYEstado(long usuarioId, EstadoEnvio estado);
    Task<IEnumerable<Envio>> GetByUsuarioYEstadoNot(long usuarioId, EstadoEnvio estado);
}
