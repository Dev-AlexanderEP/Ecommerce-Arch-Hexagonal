using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class CategoriaRepository(MixAndMatchDbContext context)
    : GenericRepository<Categoria>(context), ICategoriaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConNombre(string nombre, long? exceptoId = null) =>
        _context.Set<Categoria>()
            .AnyAsync(c => c.NomCategoria == nombre && (exceptoId == null || c.Id != exceptoId));

    public Task<bool> TienePrendas(long categoriaId) =>
        _context.Set<Prenda>().AnyAsync(p => p.CategoriaId == categoriaId);

    public Task<bool> TieneDescuentos(long categoriaId) =>
        _context.Set<DescuentoCategoria>().AnyAsync(d => d.CategoriaId == categoriaId);
}
