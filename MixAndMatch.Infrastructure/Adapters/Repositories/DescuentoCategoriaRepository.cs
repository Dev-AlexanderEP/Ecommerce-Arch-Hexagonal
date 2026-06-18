using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class DescuentoCategoriaRepository(MixAndMatchDbContext context)
    : GenericRepository<DescuentoCategoria>(context), IDescuentoCategoriaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public async Task<int> ExpirarVencidosAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Set<DescuentoCategoria>()
            .Where(d => d.Activo && d.FechaFin.HasValue && d.FechaFin < hoy)
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Activo, false)
                .SetProperty(d => d.UpdatedAt, DateTime.UtcNow));
    }

    public async Task<int> ActivarVigentesAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Set<DescuentoCategoria>()
            .Where(d => !d.Activo && d.FechaInicio <= hoy && (!d.FechaFin.HasValue || d.FechaFin >= hoy))
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Activo, true)
                .SetProperty(d => d.UpdatedAt, DateTime.UtcNow));
    }
}
