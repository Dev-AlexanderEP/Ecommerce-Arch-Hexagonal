using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class ProveedorRepository(MixAndMatchDbContext context)
    : GenericRepository<Proveedor>(context), IProveedorRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConNombre(string nombre, long? exceptoId = null) =>
        _context.Set<Proveedor>()
            .AnyAsync(p => p.NomProveedor == nombre && (exceptoId == null || p.Id != exceptoId));

    public Task<bool> TienePrendas(long proveedorId) =>
        _context.Set<Prenda>().AnyAsync(p => p.ProveedorId == proveedorId);
}
