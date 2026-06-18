using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class MarcaRepository(MixAndMatchDbContext context)
    : GenericRepository<Marca>(context), IMarcaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConNombre(string nombre, long? exceptoId = null) =>
        _context.Set<Marca>()
            .AnyAsync(m => m.NomMarca == nombre && (exceptoId == null || m.Id != exceptoId));

    public Task<bool> TienePrendas(long marcaId) =>
        _context.Set<Prenda>().AnyAsync(p => p.MarcaId == marcaId);
}
