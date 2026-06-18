using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IGeneroRepository : IGenericRepository<Genero>
{
    Task<bool> ExisteConNombre(string nombre, long? exceptoId = null);
    Task<bool> TienePrendas(long generoId);
}
