using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class PrendaRepository(MixAndMatchDbContext context)
    : GenericRepository<Prenda>(context), IPrendaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> TieneDescuentos(long prendaId) =>
        _context.Set<DescuentoPrenda>().AnyAsync(d => d.PrendaId == prendaId);

    public Task<bool> TieneImagenes(long prendaId) =>
        _context.Set<PrendaImagen>().AnyAsync(i => i.PrendaId == prendaId);

    public Task<bool> TieneTallas(long prendaId) =>
        _context.Set<PrendaTalla>().AnyAsync(t => t.PrendaId == prendaId);

    public Task<bool> TieneResenias(long prendaId) =>
        _context.Set<Resenia>().AnyAsync(r => r.PrendaId == prendaId);
}
