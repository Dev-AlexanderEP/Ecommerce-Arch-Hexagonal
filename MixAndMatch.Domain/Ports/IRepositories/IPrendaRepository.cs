using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports.IRepositories;

public interface IPrendaRepository : IGenericRepository<Prenda>
{
    Task<bool> TieneDescuentos(long prendaId);
    Task<bool> TieneImagenes(long prendaId);
    Task<bool> TieneTallas(long prendaId);
    Task<bool> TieneResenias(long prendaId);
}
