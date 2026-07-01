using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class DescuentoUsuarioRepository(MixAndMatchDbContext context)
    : GenericRepository<DescuentoUsuario>(context), IDescuentoUsuarioRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteParaUsuario(long descuentoCodigoId, long usuarioId) =>
        _context.Set<DescuentoUsuario>()
            .AnyAsync(d => d.DescuentoCodigoId == descuentoCodigoId && d.UsuarioId == usuarioId);

    public Task<DescuentoUsuario?> BuscarPorCodigoYUsuario(long descuentoCodigoId, long usuarioId) =>
        _context.Set<DescuentoUsuario>()
            .FirstOrDefaultAsync(d => d.DescuentoCodigoId == descuentoCodigoId && d.UsuarioId == usuarioId);

    public async Task<(IEnumerable<DescuentoUsuario> Items, int TotalCount)> GetPagedByUsuario(long usuarioId, int page, int pageSize)
    {
        var query = _context.Set<DescuentoUsuario>()
            .AsNoTracking()
            .Where(d => d.UsuarioId == usuarioId)
            .OrderByDescending(d => d.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
