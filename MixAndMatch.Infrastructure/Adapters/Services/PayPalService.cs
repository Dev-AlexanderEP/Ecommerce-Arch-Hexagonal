using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class PayPalService : IPayPalGatewayService
{
    private readonly HttpClient _http;
    private readonly PayPalSettings _settings;

    // El cliente HTTP tipado es transient (una instancia nueva por request), así que el token
    // OAuth se cachea a nivel estático para no pedir uno nuevo en cada llamada.
    private static string? _cachedAccessToken;
    private static DateTime _accessTokenExpiresAt = DateTime.MinValue;
    private static readonly SemaphoreSlim _tokenLock = new(1, 1);

    public PayPalService(HttpClient http, IOptions<PayPalSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    public async Task<string> ObtenerClientToken()
    {
        var accessToken = await ObtenerAccessToken();

        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/identity/generate-token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("client_token").GetString()!;
    }

    public async Task<PayPalOrdenResultado> CrearOrden(decimal montoSoles, string externalReference)
    {
        var accessToken = await ObtenerAccessToken();
        var montoUsd = Math.Round(montoSoles / _settings.TipoCambioUsd, 2);

        var payload = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    custom_id = externalReference,
                    amount = new
                    {
                        currency_code = "USD",
                        value = montoUsd.ToString("F2", CultureInfo.InvariantCulture)
                    }
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(payload);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await MapearResultado(response);
    }

    public async Task<PayPalOrdenResultado> CapturarOrden(string orderId)
    {
        var accessToken = await ObtenerAccessToken();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/v2/checkout/orders/{orderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(new { });

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await MapearResultado(response);
    }

    public async Task<bool> ValidarFirmaWebhook(
        string transmissionId,
        string transmissionTime,
        string certUrl,
        string authAlgo,
        string transmissionSig,
        string webhookEventBody)
    {
        var accessToken = await ObtenerAccessToken();

        var payload = new
        {
            transmission_id = transmissionId,
            transmission_time = transmissionTime,
            cert_url = certUrl,
            auth_algo = authAlgo,
            transmission_sig = transmissionSig,
            webhook_id = _settings.WebhookId,
            webhook_event = JsonDocument.Parse(webhookEventBody).RootElement
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/notifications/verify-webhook-signature");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(payload);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("verification_status").GetString() == "SUCCESS";
    }

    private async Task<string> ObtenerAccessToken()
    {
        if (_cachedAccessToken is not null && DateTime.UtcNow < _accessTokenExpiresAt)
        {
            return _cachedAccessToken;
        }

        await _tokenLock.WaitAsync();
        try
        {
            if (_cachedAccessToken is not null && DateTime.UtcNow < _accessTokenExpiresAt)
            {
                return _cachedAccessToken;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = body.GetProperty("access_token").GetString()!;
            var expiresIn = body.GetProperty("expires_in").GetInt32();

            _cachedAccessToken = token;
            _accessTokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            return token;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private static async Task<PayPalOrdenResultado> MapearResultado(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return new PayPalOrdenResultado(
            OrderId: body.GetProperty("id").GetString()!,
            Status: body.GetProperty("status").GetString()!);
    }
}
