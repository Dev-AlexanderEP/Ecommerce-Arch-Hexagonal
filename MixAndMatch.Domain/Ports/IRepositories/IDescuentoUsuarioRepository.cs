using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IDescuentoUsuarioRepository : IGenericRepository<DescuentoUsuario>
{
    Task<bool> ExisteParaUsuario(long descuentoCodigoId, long usuarioId);
    Task<DescuentoUsuario?> BuscarPorCodigoYUsuario(long descuentoCodigoId, long usuarioId);
    Task<(IEnumerable<DescuentoUsuario> Items, int TotalCount)> GetPagedByUsuario(long usuarioId, int page, int pageSize);
}
