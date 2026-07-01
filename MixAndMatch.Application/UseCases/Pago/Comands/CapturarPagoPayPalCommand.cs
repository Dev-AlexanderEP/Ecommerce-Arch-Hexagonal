using System.Text.Json.Serialization;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.DTOs.MetodoPago;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class CapturarPagoPayPalCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    public required long PagoId { get; set; }
    public required string OrderId { get; set; }

    [JsonIgnore] public long SolicitanteId { get; set; }
}

public class CapturarPagoPayPalCommandHandler(IUnitOfWork _uow, IPayPalGatewayService _gateway, IMediator _mediator)
    : IRequestHandler<CapturarPagoPayPalCommand, ApiResponse<PagoResponseDto>>
{
    public async Task<ApiResponse<PagoResponseDto>> Handle(CapturarPagoPayPalCommand request, CancellationToken cancellationToken)
    {
        var pago = await _uow.Repository<PagoEntity>().GetById(request.PagoId);
        if (pago is null)
        {
            return ApiResponse<PagoResponseDto>.Fail($"Pago no encontrado para id {request.PagoId}.", ErrorType.Validation);
        }

        var venta = await _uow.Ventas.GetById(pago.VentaId);
        if (venta is null || venta.UsuarioId != request.SolicitanteId)
        {
            return ApiResponse<PagoResponseDto>.Fail("No tienes acceso a este pago.", ErrorType.Forbidden);
        }

        if (pago.Estado != EstadoPago.PENDIENTE)
        {
            return ApiResponse<PagoResponseDto>.Fail("El pago ya fue procesado.", ErrorType.Conflict);
        }

        PayPalOrdenResultado resultado;
        try
        {
            resultado = await _gateway.CapturarOrden(request.OrderId);
        }
        catch (Exception)
        {
            return ApiResponse<PagoResponseDto>.Fail("No se pudo capturar el pago con PayPal.", ErrorType.Validation);
        }

        if (resultado.Status == "COMPLETED")
        {
            return await _mediator.Send(new ConfirmarPagoCommand { PagoId = pago.Id }, cancellationToken);
        }

        pago.Estado = EstadoPago.FALLIDO;
        pago.UpdatedAt = DateTime.UtcNow;
        await _uow.Repository<PagoEntity>().Update(pago);
        await _uow.Complete();

        return ApiResponse<PagoResponseDto>.Fail($"Pago rechazado por PayPal: {resultado.Status}", ErrorType.Validation);
    }
}
