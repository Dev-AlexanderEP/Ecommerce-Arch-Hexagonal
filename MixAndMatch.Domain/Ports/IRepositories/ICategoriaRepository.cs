using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface ICategoriaRepository : IGenericRepository<Categoria>
{
    Task<bool> ExisteConNombre(string nombre, long? exceptoId = null);
    Task<bool> TienePrendas(long categoriaId);
    Task<bool> TieneDescuentos(long categoriaId);
}
