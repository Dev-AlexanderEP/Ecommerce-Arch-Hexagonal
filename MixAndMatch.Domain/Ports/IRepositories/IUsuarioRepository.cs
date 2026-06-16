using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmail(string email);
    Task<bool> ExistsByEmail(string email);
    Task<bool> ExistsByNombreUsuario(string nombreUsuario);
}
