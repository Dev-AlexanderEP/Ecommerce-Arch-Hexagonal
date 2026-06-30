using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Error;
using MercadoPago.Resource.Payment;
using MercadoPago.Webhook;
using Microsoft.Extensions.Options;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class MercadoPagoService : IPagoGatewayService
{
    private readonly MercadoPagoSettings _settings;

    public MercadoPagoService(IOptions<MercadoPagoSettings> options)
    {
        _settings = options.Value;
        MercadoPagoConfig.AccessToken = _settings.AccessToken;
    }

    public async Task<PagoGatewayResultado> CrearPago(CrearPagoGatewayRequest request)
    {
        var createRequest = new PaymentCreateRequest
        {
            TransactionAmount = request.Monto,
            Token = request.Token,
            PaymentMethodId = request.PaymentMethodId,
            IssuerId = request.IssuerId,
            Installments = request.Installments,
            ExternalReference = request.ExternalReference,
            Payer = new PaymentPayerRequest
            {
                Email = request.PayerEmail,
                Identification = request.IdentificacionTipo is not null && request.IdentificacionNumero is not null
                    ? new IdentificationRequest
                    {
                        Type = request.IdentificacionTipo,
                        Number = request.IdentificacionNumero
                    }
                    : null
            }
        };

        var client = new PaymentClient();
        Payment payment = await client.CreateAsync(createRequest);

        return MapearResultado(payment);
    }

    public async Task<PagoGatewayResultado> ObtenerPago(string gatewayPaymentId)
    {
        var client = new PaymentClient();
        Payment payment = await client.GetAsync(long.Parse(gatewayPaymentId));

        return MapearResultado(payment);
    }

    public bool ValidarFirmaWebhook(string xSignature, string xRequestId, string dataId)
    {
        try
        {
            WebhookSignatureValidator.Validate(xSignature, xRequestId, dataId, _settings.WebhookSecret);
            return true;
        }
        catch (InvalidWebhookSignatureException)
        {
            return false;
        }
    }

    private static PagoGatewayResultado MapearResultado(Payment payment) => new(
        GatewayPaymentId: payment.Id.ToString()!,
        Status: payment.Status,
        StatusDetail: payment.StatusDetail,
        ExternalReference: payment.ExternalReference);
}
