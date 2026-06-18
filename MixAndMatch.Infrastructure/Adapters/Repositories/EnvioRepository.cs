using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class EnvioRepository(MixAndMatchDbContext context)
    : GenericRepository<Envio>(context), IEnvioRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteParaVenta(long ventaId, long? exceptoId = null) =>
        _context.Set<Envio>()
            .AnyAsync(e => e.VentaId == ventaId && (exceptoId == null || e.Id != exceptoId));
}
