using MixAndMatch.Domain.Entities;

namespace MixAndMatch.Domain.Ports;

public interface IReseniaRepository
{
    Task<(IReadOnlyList<Resenia> Items, int TotalCount)> GetPaginatedByPrendaIdAsync(
        long prendaId,
        int page,
        int pageSize);

    Task<IReadOnlyList<Resenia>> GetByUsuarioIdAsync(long usuarioId);

    Task<Resenia?> GetByPrendaAndUsuarioAsync(long prendaId, long usuarioId);

    Task<decimal> GetPromedioByPrendaIdAsync(long prendaId);
}
