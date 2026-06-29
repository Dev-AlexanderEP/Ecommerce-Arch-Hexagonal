using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IDescuentoCodigoRepository : IGenericRepository<DescuentoCodigo>
{
    Task<bool> ExisteConCodigo(string codigo, long? exceptoId = null);
    Task<bool> TieneUsos(long descuentoCodigoId);
    Task<DescuentoCodigo?> BuscarPorCodigo(string codigo);

    // Jobs de mantenimiento
    Task<int> ExpirarVencidosAsync();
    Task<int> ActivarVigentesAsync();
    Task<int> ExpirarPorUsoMaximoAsync();
}
