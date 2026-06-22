using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;

namespace MixAndMatch.Domain.Ports;

public interface IReseniaRepository : IGenericRepository<Resenia>
{
    Task<bool> ExisteResenia(long prendaId, long usuarioId);

    Task<IReadOnlyList<ReseniaByPrendaItemDto>> GetByPrendaAsync(long prendaId);

    Task<(IEnumerable<Resenia> Items, int TotalCount)> BuscarAsync(
        long? prendaId,
        long? usuarioId,
        int? calificacion,
        int page,
        int pageSize);
}
