namespace MixAndMatch.Domain.Ports.IServices;

public record PayPalOrdenResultado(string OrderId, string Status);

public interface IPayPalGatewayService
{
    /// Client token de corta duración que el frontend necesita para inicializar Advanced Card Fields.
    Task<string> ObtenerClientToken();

    /// Crea la orden en PayPal (intent CAPTURE) por el monto en soles, convertido a USD internamente.
    Task<PayPalOrdenResultado> CrearOrden(decimal montoSoles, string externalReference);

    /// Captura una orden previamente aprobada por el comprador.
    Task<PayPalOrdenResultado> CapturarOrden(string orderId);

    /// Valida la firma de un webhook de PayPal contra el WebhookId configurado.
    Task<bool> ValidarFirmaWebhook(
        string transmissionId,
        string transmissionTime,
        string certUrl,
        string authAlgo,
        string transmissionSig,
        string webhookEventBody);
}
