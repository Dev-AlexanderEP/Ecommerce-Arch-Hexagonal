using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class GeneroRepository(MixAndMatchDbContext context)
    : GenericRepository<Genero>(context), IGeneroRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConNombre(string nombre, long? exceptoId = null) =>
        _context.Set<Genero>()
            .AnyAsync(g => g.NomGenero == nombre && (exceptoId == null || g.Id != exceptoId));

    public Task<bool> TienePrendas(long generoId) =>
        _context.Set<Prenda>().AnyAsync(p => p.GeneroId == generoId);
}
