using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.DTOs.Resenias;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class ReseniaRepository(MixAndMatchDbContext context)
    : GenericRepository<Resenia>(context), IReseniaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteResenia(long prendaId, long usuarioId) =>
        _context.Resenia.AnyAsync(r => r.PrendaId == prendaId && r.UsuarioId == usuarioId);

    public async Task<IReadOnlyList<ReseniaByPrendaItemDto>> GetByPrendaAsync(long prendaId)
    {
        return await _context.Resenia
            .AsNoTracking()
            .Where(r => r.PrendaId == prendaId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReseniaByPrendaItemDto
            {
                UsuarioId = r.UsuarioId,
                NombreUsuario = r.Usuario.NombreUsuario,
                PuntajeEstrellas = r.Calificacion,
                // CantidadResenias: total de resenias escritas por ese usuario (subconsulta correlacionada).
                CantidadResenias = _context.Resenia.Count(x => x.UsuarioId == r.UsuarioId),
                Comentario = r.Comentario,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<(IEnumerable<Resenia> Items, int TotalCount)> BuscarAsync(
        long? prendaId,
        long? usuarioId,
        int? calificacion,
        int page,
        int pageSize)
    {
        var query = _context.Resenia.AsNoTracking();

        if (prendaId.HasValue)
            query = query.Where(r => r.PrendaId == prendaId.Value);

        if (usuarioId.HasValue)
            query = query.Where(r => r.UsuarioId == usuarioId.Value);

        if (calificacion.HasValue)
            query = query.Where(r => r.Calificacion == calificacion.Value);

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
