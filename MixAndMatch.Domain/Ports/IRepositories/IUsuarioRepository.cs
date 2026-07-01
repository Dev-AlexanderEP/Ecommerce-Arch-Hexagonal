using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmail(string email);
    Task<bool> ExistsByEmail(string email, long? exceptoId = null);
    Task<bool> ExistsByNombreUsuario(string nombreUsuario, long? exceptoId = null);
    Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedConFiltro(
        string? nombre, string? email, string? rol, bool? activo, int page, int pageSize);
}
