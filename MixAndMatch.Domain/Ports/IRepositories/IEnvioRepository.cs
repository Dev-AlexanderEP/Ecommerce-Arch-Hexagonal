using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IEnvioRepository : IGenericRepository<Envio>
{
    Task<bool> ExisteParaVenta(long ventaId, long? exceptoId = null);
}
