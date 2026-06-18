using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IMetodoPagoRepository : IGenericRepository<MetodoPago>
{
    Task<bool> ExisteConTipo(string tipoPago, long? exceptoId = null);
    Task<bool> TienePagos(long metodoId);
}
