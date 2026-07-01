using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.Services;

public class ConfirmacionPagoService(IUnitOfWork _uow) : IConfirmacionPagoService
{
    public async Task<ApiResponse<PagoResponseDto>> ConfirmarAsync(long pagoId, CancellationToken ct = default)
    {
        var pago = await _uow.Repository<PagoEntity>().GetById(pagoId);
        if (pago is null)
            return ApiResponse<PagoResponseDto>.Fail($"Pago no encontrado para id {pagoId}.", ErrorType.Validation);

        if (pago.Estado != EstadoPago.PENDIENTE)
            return ApiResponse<PagoResponseDto>.Fail("El pago ya fue procesado.", ErrorType.Conflict);

        var venta = await _uow.Ventas.GetById(pago.VentaId);
        if (venta is null)
            return ApiResponse<PagoResponseDto>.Fail($"Venta no encontrada para id {pago.VentaId}.", ErrorType.Validation);

        pago.Estado = EstadoPago.COMPLETADO;
        pago.UpdatedAt = DateTime.UtcNow;
        await _uow.Repository<PagoEntity>().Update(pago);

        venta.Estado = EstadoVenta.PAGADO;
        venta.UpdatedAt = DateTime.UtcNow;
        await _uow.Ventas.Update(venta);

        var carritosActivos = await _uow.Carritos.BuscarAbiertosPorUsuarioId(venta.UsuarioId);
        foreach (var carrito in carritosActivos)
        {
            carrito.Estado = EstadoCarrito.CERRADO;
            carrito.UpdatedAt = DateTime.UtcNow;
            await _uow.Carritos.Update(carrito);
        }

        await _uow.Complete();

        return ApiResponse<PagoResponseDto>.Ok(new PagoResponseDto
        {
            Id            = pago.Id,
            VentaId       = pago.VentaId,
            MetodoId      = pago.MetodoId,
            Monto         = pago.Monto,
            Estado        = pago.Estado.ToString(),
            FechaCreacion = pago.FechaCreacion,
            UpdatedAt     = pago.UpdatedAt
        });
    }
}
