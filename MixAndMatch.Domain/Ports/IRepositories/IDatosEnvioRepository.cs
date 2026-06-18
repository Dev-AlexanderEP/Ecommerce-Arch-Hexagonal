using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IDatosEnvioRepository : IGenericRepository<DatosEnvio>
{
    Task<bool> ExistePorUsuario(long usuarioId);
}
