using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IDescuentoCodigoRepository : IGenericRepository<DescuentoCodigo>
{
    Task<bool> ExisteConCodigo(string codigo, long? exceptoId = null);
    Task<bool> TieneUsos(long descuentoCodigoId);
}
