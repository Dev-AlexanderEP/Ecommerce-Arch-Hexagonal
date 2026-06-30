using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

// Marca un pago PENDIENTE como COMPLETADO y la venta asociada como PAGADO.
// Lo dispara el backend (auto-confirmación de tarjeta simbólica) o, a futuro, el webhook de PayPal/MercadoPago.
public class ConfirmarPagoCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    public required long PagoId { get; set; }
}

public class ConfirmarPagoCommandHandler(IUnitOfWork _uow)
    : IRequestHandler<ConfirmarPagoCommand, ApiResponse<PagoResponseDto>>
{
    public async Task<ApiResponse<PagoResponseDto>> Handle(ConfirmarPagoCommand request, CancellationToken cancellationToken)
    {
        var pago = await _uow.Repository<PagoEntity>().GetById(request.PagoId);
        if (pago is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Pago no encontrado para id {request.PagoId}.", ErrorType.Validation);
        }

        if (pago.Estado != EstadoPago.PENDIENTE)
        {
            return ApiResponse<PagoResponseDto>.Fail("El pago ya fue procesado.", ErrorType.Conflict);
        }

        var venta = await _uow.Ventas.GetById(pago.VentaId);
        if (venta is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Venta no encontrada para id {pago.VentaId}.", ErrorType.Validation);
        }

        pago.Estado = EstadoPago.COMPLETADO;
        pago.UpdatedAt = DateTime.UtcNow;
        await _uow.Repository<PagoEntity>().Update(pago);

        venta.Estado = EstadoVenta.PAGADO;
        venta.UpdatedAt = DateTime.UtcNow;
        await _uow.Ventas.Update(venta);

        await _uow.Complete();

        return ApiResponse<PagoResponseDto>.Ok(new PagoResponseDto
        {
            Id = pago.Id,
            VentaId = pago.VentaId,
            MetodoId = pago.MetodoId,
            Monto = pago.Monto,
            Estado = pago.Estado.ToString(),
            FechaCreacion = pago.FechaCreacion,
            UpdatedAt = pago.UpdatedAt
        });
    }
}
