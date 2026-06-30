using MediatR;
using MixAndMatch.Application.Common;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Ports.IRepositories;
using MixAndMatch.Domain.Ports.IServices;
using PagoEntity = MixAndMatch.Domain.Entities.Pago;

namespace MixAndMatch.Application.UseCases.Pago.Commands;

public class ProcesarWebhookMercadoPagoCommand : IRequest<ApiResponse<string>>
{
    public required string XSignature { get; set; }
    public required string XRequestId { get; set; }
    public required string DataId { get; set; }
}

public class ProcesarWebhookMercadoPagoCommandHandler(IUnitOfWork _uow, IPagoGatewayService _gateway, IMediator _mediator)
    : IRequestHandler<ProcesarWebhookMercadoPagoCommand, ApiResponse<string>>
{
    private const string PrefijoReferencia = "pago-";

    public async Task<ApiResponse<string>> Handle(ProcesarWebhookMercadoPagoCommand request, CancellationToken cancellationToken)
    {
        // Nunca confiar en el body del webhook: validar la firma y luego re-consultar el pago real.
        if (!_gateway.ValidarFirmaWebhook(request.XSignature, request.XRequestId, request.DataId))
        {
            return ApiResponse<string>.Fail("Firma de webhook inválida.", ErrorType.Unauthorized);
        }

        PagoGatewayResultado resultado;
        try
        {
            resultado = await _gateway.ObtenerPago(request.DataId);
        }
        catch (Exception)
        {
            // Puede ser una notificación de prueba (botón "Simular notificación") u otro pago
            // ya inexistente del lado de Mercado Pago: no es un error nuestro, solo se ignora.
            return ApiResponse<string>.Ok("Notificación ignorada: no se pudo obtener el pago en Mercado Pago.");
        }

        // El external_reference que nosotros mandamos al crear el pago ("pago-{Id}") es lo que nos
        // permite ubicar nuestro Pago sin tener que guardar el id de Mercado Pago en la BD.
        if (resultado.ExternalReference is null
            || !resultado.ExternalReference.StartsWith(PrefijoReferencia, StringComparison.Ordinal)
            || !long.TryParse(resultado.ExternalReference[PrefijoReferencia.Length..], out var pagoId))
        {
            return ApiResponse<string>.Ok("Notificación ignorada: referencia externa desconocida.");
        }

        var pago = await _uow.Repository<PagoEntity>().GetById(pagoId);
        if (pago is null)
        {
            return ApiResponse<string>.Ok("Notificación ignorada: pago no encontrado.");
        }

        // Idempotencia: Mercado Pago puede reintentar la misma notificación varias veces.
        if (pago.Estado != EstadoPago.PENDIENTE)
        {
            return ApiResponse<string>.Ok("Pago ya procesado.");
        }

        switch (resultado.Status)
        {
            case "approved":
                await _mediator.Send(new ConfirmarPagoCommand { PagoId = pago.Id }, cancellationToken);
                break;

            case "rejected":
            case "cancelled":
                pago.Estado = EstadoPago.FALLIDO;
                pago.UpdatedAt = DateTime.UtcNow;
                await _uow.Repository<PagoEntity>().Update(pago);
                await _uow.Complete();
                break;

            default:
                // "in_process" / "pending": todavía no hay nada que confirmar.
                break;
        }

        return ApiResponse<string>.Ok("Notificación procesada.");
    }
}
