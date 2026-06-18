using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IMarcaRepository : IGenericRepository<Marca>
{
    Task<bool> ExisteConNombre(string nombre, long? exceptoId = null);
    Task<bool> TienePrendas(long marcaId);
}
