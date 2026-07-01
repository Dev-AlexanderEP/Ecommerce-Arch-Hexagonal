namespace MixAndMatch.Infrastructure.Configuration;

public class PayPalSettings
{
    public const string SectionName = "PayPal";
    public string ClientId       { get; set; } = null!;
    public string ClientSecret   { get; set; } = null!;
    public string BaseUrl        { get; set; } = "https://api-m.sandbox.paypal.com";
    public string WebhookId      { get; set; } = null!;
    public decimal TipoCambioUsd { get; set; } = 3.7m;
}
