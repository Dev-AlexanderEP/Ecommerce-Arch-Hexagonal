using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters;

public class EnvioRepository(MixAndMatchDbContext context)
    : GenericRepository<Envio>(context), IEnvioRepository
{
    private readonly MixAndMatchDbContext _context = context;

    public Task<bool> ExisteParaVenta(long ventaId, long? exceptoId = null) =>
        _context.Set<Envio>()
            .AnyAsync(e => e.VentaId == ventaId && (exceptoId == null || e.Id != exceptoId));

    public Task<Envio?> FindByTrackingNumber(string trackingNumber) =>
        _context.Set<Envio>()
            .FirstOrDefaultAsync(e => e.TrackingNumber == trackingNumber);

    public Task<Envio?> FindByTrackingNumberDetallado(string trackingNumber) =>
        _context.Set<Envio>()
            .Where(e => e.TrackingNumber == trackingNumber)
            .Include(e => e.DatosEnvio)
            .Include(e => e.Venta)
                .ThenInclude(v => v.VentasDetalles)
                    .ThenInclude(d => d.PrendaTalla)
                        .ThenInclude(pt => pt.Prenda)
                            .ThenInclude(p => p.PrendaImagens)
            .Include(e => e.Venta)
                .ThenInclude(v => v.VentasDetalles)
                    .ThenInclude(d => d.PrendaTalla)
                        .ThenInclude(pt => pt.Talla)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Envio>> GetByUsuarioYEstado(long usuarioId, EstadoEnvio estado) =>
        await _context.Set<Envio>()
            .Where(e => e.Venta.UsuarioId == usuarioId && e.Estado == estado)
            .ToListAsync();

    public async Task<IEnumerable<Envio>> GetByUsuarioYEstadoNot(long usuarioId, EstadoEnvio estado) =>
        await _context.Set<Envio>()
            .Where(e => e.Venta.UsuarioId == usuarioId && e.Estado != estado)
            .ToListAsync();
}
