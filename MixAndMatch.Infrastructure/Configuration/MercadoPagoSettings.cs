namespace MixAndMatch.Infrastructure.Configuration;

public class MercadoPagoSettings
{
    public const string SectionName = "MercadoPago";
    public string AccessToken   { get; set; } = null!;
    public string PublicKey     { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
}
