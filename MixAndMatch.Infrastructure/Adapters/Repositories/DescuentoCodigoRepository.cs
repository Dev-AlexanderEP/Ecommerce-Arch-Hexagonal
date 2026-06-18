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
}
