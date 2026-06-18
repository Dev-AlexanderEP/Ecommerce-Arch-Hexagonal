using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface ITallaRepository : IGenericRepository<Talla>
{
    Task<bool> ExisteConNombre(string nombre, long? exceptoId = null);
    Task<bool> TienePrendaTallas(long tallaId);
}
