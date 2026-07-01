namespace MixAndMatch.Infrastructure.Configuration;

public class DeepSeekSettings
{
    public const string SectionName = "DeepSeek";
    public string ApiKey  { get; set; } = null!;
    public string BaseUrl { get; set; } = "https://api.deepseek.com";
    public string Model   { get; set; } = "deepseek-chat";
}
