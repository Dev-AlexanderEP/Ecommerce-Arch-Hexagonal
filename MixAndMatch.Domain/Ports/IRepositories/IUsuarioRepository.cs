using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmail(string email);
    Task<bool> ExistsByEmail(string email, long? exceptoId = null);
    Task<bool> ExistsByNombreUsuario(string nombreUsuario, long? exceptoId = null);
}
