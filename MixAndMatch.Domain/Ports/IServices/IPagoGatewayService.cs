namespace MixAndMatch.Domain.Ports.IServices;

public record CrearPagoGatewayRequest(
    decimal Monto,
    string Token,
    string PaymentMethodId,
    string? IssuerId,
    int Installments,
    string PayerEmail,
    string? IdentificacionTipo,
    string? IdentificacionNumero,
    string ExternalReference);

// ExternalReference es el "pago-{Id}" que nosotros mandamos al crear el pago;
// Mercado Pago lo devuelve tal cual en cualquier consulta, así el webhook
// (que solo trae el id del pago en MP) puede ubicar nuestro Pago sin guardar nada nuevo en la BD.
public record PagoGatewayResultado(
    string GatewayPaymentId,
    string Status,
    string? StatusDetail,
    string? ExternalReference);

public interface IPagoGatewayService
{
    Task<PagoGatewayResultado> CrearPago(CrearPagoGatewayRequest request);
    Task<PagoGatewayResultado> ObtenerPago(string gatewayPaymentId);

    /// Valida la firma x-signature de un webhook de Mercado Pago contra el WebhookSecret configurado.
    bool ValidarFirmaWebhook(string xSignature, string xRequestId, string dataId);
}
