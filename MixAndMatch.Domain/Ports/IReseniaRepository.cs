using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Domain.Ports;

public interface IReseniaRepository : IGenericRepository<Resenia>
{
    Task<(IReadOnlyList<Resenia> Items, int TotalCount)> GetPaginatedByPrendaIdAsync(
        long prendaId,
        int page,
        int pageSize);

    Task<IReadOnlyList<Resenia>> GetByUsuarioIdAsync(long usuarioId);

    Task<Resenia?> GetByPrendaAndUsuarioAsync(long prendaId, long usuarioId);

    Task<decimal> GetPromedioByPrendaIdAsync(long prendaId);
}
