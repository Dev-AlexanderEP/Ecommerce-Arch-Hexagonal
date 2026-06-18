using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class DescuentoCodigoRepository(MixAndMatchDbContext context)
    : GenericRepository<DescuentoCodigo>(context), IDescuentoCodigoRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConCodigo(string codigo, long? exceptoId = null) =>
        _context.Set<DescuentoCodigo>()
            .AnyAsync(d => d.Codigo == codigo && (exceptoId == null || d.Id != exceptoId));

    public Task<bool> TieneUsos(long descuentoCodigoId) =>
        _context.Set<DescuentoUsuario>().AnyAsync(u => u.DescuentoCodigoId == descuentoCodigoId);

    public async Task<int> ExpirarVencidosAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Set<DescuentoCodigo>()
            .Where(d => d.Activo && d.FechaFin.HasValue && d.FechaFin < hoy)
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Activo, false)
                .SetProperty(d => d.UpdatedAt, DateTime.UtcNow));
    }

    public async Task<int> ActivarVigentesAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Set<DescuentoCodigo>()
            .Where(d => !d.Activo && d.FechaInicio <= hoy && (!d.FechaFin.HasValue || d.FechaFin >= hoy))
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Activo, true)
                .SetProperty(d => d.UpdatedAt, DateTime.UtcNow));
    }

    public async Task<int> ExpirarPorUsoMaximoAsync()
    {
        // Carga solo los IDs (conjunto pequeño) para evitar subquery correlacionada en ExecuteUpdateAsync.
        var idsAgotados = await _context.Set<DescuentoCodigo>()
            .Where(d => d.Activo)
            .Join(
                _context.Set<DescuentoUsuario>()
                    .GroupBy(u => u.DescuentoCodigoId)
                    .Select(g => new { CodigoId = g.Key, Usos = g.Count() }),
                d => d.Id,
                g => g.CodigoId,
                (d, g) => new { d.Id, d.UsoMaximo, g.Usos })
            .Where(x => x.Usos >= x.UsoMaximo)
            .Select(x => x.Id)
            .ToListAsync();

        if (idsAgotados.Count == 0) return 0;

        return await _context.Set<DescuentoCodigo>()
            .Where(d => idsAgotados.Contains(d.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Activo, false)
                .SetProperty(d => d.UpdatedAt, DateTime.UtcNow));
    }
}
