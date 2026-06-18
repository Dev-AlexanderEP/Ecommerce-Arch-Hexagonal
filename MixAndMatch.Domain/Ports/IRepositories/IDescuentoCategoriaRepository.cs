using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IDescuentoCategoriaRepository : IGenericRepository<DescuentoCategoria>
{
    Task<int> ExpirarVencidosAsync();
    Task<int> ActivarVigentesAsync();
}
