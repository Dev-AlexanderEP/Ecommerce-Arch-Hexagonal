using System.Text.Json;
using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class ProcesarWebhookPayPalCommand : IRequest<ApiResponse<string>>
{
    public required string TransmissionId { get; set; }
    public required string TransmissionTime { get; set; }
    public required string CertUrl { get; set; }
    public required string AuthAlgo { get; set; }
    public required string TransmissionSig { get; set; }
    public required string EventBody { get; set; }
}

public class ProcesarWebhookPayPalCommandHandler(IUnitOfWork _uow, IPayPalGatewayService _gateway, IMediator _mediator)
    : IRequestHandler<ProcesarWebhookPayPalCommand, ApiResponse<string>>
{
    private const string PrefijoReferencia = "pago-";

    public async Task<ApiResponse<string>> Handle(ProcesarWebhookPayPalCommand request, CancellationToken cancellationToken)
    {
        bool firmaValida;
        try
        {
            firmaValida = await _gateway.ValidarFirmaWebhook(
                request.TransmissionId,
                request.TransmissionTime,
                request.CertUrl,
                request.AuthAlgo,
                request.TransmissionSig,
                request.EventBody);
        }
        catch (Exception)
        {
            return ApiResponse<string>.Fail("No se pudo validar la firma del webhook.", ErrorType.Unauthorized);
        }

        if (!firmaValida)
        {
            return ApiResponse<string>.Fail("Firma de webhook inválida.", ErrorType.Unauthorized);
        }

        using var doc = JsonDocument.Parse(request.EventBody);
        var root = doc.RootElement;

        var eventType = root.TryGetProperty("event_type", out var eventTypeProp) ? eventTypeProp.GetString() : null;
        if (eventType != "PAYMENT.CAPTURE.COMPLETED")
        {
            return ApiResponse<string>.Ok("Notificación ignorada: evento no relevante.");
        }

        if (!root.TryGetProperty("resource", out var resource)
            || !resource.TryGetProperty("custom_id", out var customIdProp)
            || customIdProp.GetString() is not string customId
            || !customId.StartsWith(PrefijoReferencia, StringComparison.Ordinal)
            || !long.TryParse(customId[PrefijoReferencia.Length..], out var pagoId))
        {
            return ApiResponse<string>.Ok("Notificación ignorada: referencia externa desconocida.");
        }

        var pago = await _uow.Repository<PagoEntity>().GetById(pagoId);
        if (pago is null)
        {
            return ApiResponse<string>.Ok("Notificación ignorada: pago no encontrado.");
        }

        // Idempotencia: PayPal puede reintentar la misma notificación varias veces.
        if (pago.Estado != EstadoPago.PENDIENTE)
        {
            return ApiResponse<string>.Ok("Pago ya procesado.");
        }

        await _mediator.Send(new ConfirmarPagoCommand { PagoId = pago.Id }, cancellationToken);

        return ApiResponse<string>.Ok("Notificación procesada.");
    }
}
