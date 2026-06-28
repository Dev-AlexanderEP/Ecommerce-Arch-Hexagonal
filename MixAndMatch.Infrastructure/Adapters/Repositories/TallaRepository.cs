using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class TallaRepository(MixAndMatchDbContext context)
    : GenericRepository<Talla>(context), ITallaRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConNombre(string nombre, long? exceptoId = null) =>
        _context.Set<Talla>()
            .AnyAsync(t => t.NomTalla == nombre && (exceptoId == null || t.Id != exceptoId));

    public Task<bool> TienePrendaTallas(long tallaId) =>
        _context.Set<PrendaTalla>().AnyAsync(pt => pt.TallaId == tallaId);

    public Task<Talla?> BuscarPorNombre(string nomTalla) =>
        _context.Set<Talla>()
            .FirstOrDefaultAsync(t => t.NomTalla.ToLower() == nomTalla.ToLower());
}
