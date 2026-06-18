using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IDescuentoPrendaRepository : IGenericRepository<DescuentoPrenda>
{
    Task<int> ExpirarVencidosAsync();
    Task<int> ActivarVigentesAsync();
}
