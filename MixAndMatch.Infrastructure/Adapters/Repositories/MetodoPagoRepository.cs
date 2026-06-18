using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class MetodoPagoRepository(MixAndMatchDbContext context)
    : GenericRepository<MetodoPago>(context), IMetodoPagoRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteConTipo(string tipoPago, long? exceptoId = null) =>
        _context.Set<MetodoPago>()
            .AnyAsync(m => m.TipoPago == tipoPago && (exceptoId == null || m.Id != exceptoId));

    public Task<bool> TienePagos(long metodoId) =>
        _context.Set<Pago>().AnyAsync(p => p.MetodoId == metodoId);
}
