using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Application.Services;
using MixAndMatch.Domain.DTOs.MetodoPago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

// Marca un pago PENDIENTE como COMPLETADO y la venta asociada como PAGADO.
// Lo dispara el backend (auto-confirmación de tarjeta simbólica) o el webhook de PayPal/MercadoPago.
public class ConfirmarPagoCommand : IRequest<ApiResponse<PagoResponseDto>>
{
    public required long PagoId { get; set; }
}

public class ConfirmarPagoCommandHandler(IConfirmacionPagoService _confirmacion)
    : IRequestHandler<ConfirmarPagoCommand, ApiResponse<PagoResponseDto>>
{
    public Task<ApiResponse<PagoResponseDto>> Handle(ConfirmarPagoCommand request, CancellationToken cancellationToken) =>
        _confirmacion.ConfirmarAsync(request.PagoId, cancellationToken);
}
