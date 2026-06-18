using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class ReseniaRepository(MixAndMatchDbContext context)
    : GenericRepository<Resenia>(context), IReseniaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public async Task<(IReadOnlyList<Resenia> Items, int TotalCount)> GetPaginatedByPrendaIdAsync(
        long prendaId,
        int page,
        int pageSize)
    {
        var query = _context.Resenia
            .AsNoTracking()
            .Where(r => r.PrendaId == prendaId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Resenia>> GetByUsuarioIdAsync(long usuarioId)
    {
        return await _context.Resenia
            .AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Resenia?> GetByPrendaAndUsuarioAsync(long prendaId, long usuarioId)
    {
        return await _context.Resenia
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.PrendaId == prendaId && r.UsuarioId == usuarioId);
    }

    public async Task<decimal> GetPromedioByPrendaIdAsync(long prendaId)
    {
        var promedio = await _context.Resenia
            .AsNoTracking()
            .Where(r => r.PrendaId == prendaId)
            .Select(r => (decimal?)r.Calificacion)
            .AverageAsync();

        return promedio ?? 0m;
    }
}
