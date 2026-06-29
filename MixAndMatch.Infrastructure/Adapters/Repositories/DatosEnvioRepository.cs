using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class DatosEnvioRepository(MixAndMatchDbContext context)
    : GenericRepository<DatosEnvio>(context), IDatosEnvioRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExistePorUsuario(long usuarioId) =>
        _context.Set<DatosEnvio>().AnyAsync(d => d.UsuarioId == usuarioId);

    public Task<List<DatosEnvio>> GetByUsuarioId(long usuarioId) =>
        _context.Set<DatosEnvio>()
            .Where(d => d.UsuarioId == usuarioId)
            .OrderByDescending(d => d.EsPrincipal)
            .ThenByDescending(d => d.CreatedAt)
            .ToListAsync();
}
