using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class UsuarioRepository(MixAndMatchDbContext context)
    : GenericRepository<Usuario>(context), IUsuarioRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<Usuario?> GetByEmail(string email) =>
        _context.Set<Usuario>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> ExistsByEmail(string email) =>
        _context.Set<Usuario>()
            .AnyAsync(u => u.Email == email);

    public Task<bool> ExistsByNombreUsuario(string nombreUsuario) =>
        _context.Set<Usuario>()
            .AnyAsync(u => u.NombreUsuario == nombreUsuario);
}
