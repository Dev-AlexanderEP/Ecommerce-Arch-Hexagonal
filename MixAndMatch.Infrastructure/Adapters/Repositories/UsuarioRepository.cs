using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
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

    public Task<bool> ExistsByEmail(string email, long? exceptoId = null) =>
        _context.Set<Usuario>()
            .AnyAsync(u => u.Email == email && (exceptoId == null || u.Id != exceptoId));

    public Task<bool> ExistsByNombreUsuario(string nombreUsuario, long? exceptoId = null) =>
        _context.Set<Usuario>()
            .AnyAsync(u => u.NombreUsuario == nombreUsuario && (exceptoId == null || u.Id != exceptoId));

    public Task<int> GetTotal() =>
        _context.Set<Usuario>().CountAsync();

    public async Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedConFiltro(
        string? nombre, string? email, string? rol, bool? activo, int page, int pageSize)
    {
        var query = _context.Set<Usuario>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(u => EF.Functions.ILike(u.NombreUsuario, $"%{nombre}%"));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => EF.Functions.ILike(u.Email, $"%{email}%"));

        if (!string.IsNullOrWhiteSpace(rol) && Enum.TryParse<RolUsuario>(rol, ignoreCase: true, out var rolEnum))
            query = query.Where(u => u.Rol == rolEnum);

        if (activo.HasValue)
            query = query.Where(u => u.Activo == activo.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.NombreUsuario)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
