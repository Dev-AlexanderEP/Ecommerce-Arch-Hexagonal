namespace MixAndMatch.Infrastructure.Configuration;

public class R2Settings
{
    public const string SectionName = "R2";
    public string AccountId   { get; set; } = null!;
    public string AccessKeyId { get; set; } = null!;
    public string SecretKey   { get; set; } = null!;
    public string BucketName  { get; set; } = null!;
    public string PublicUrl   { get; set; } = null!;
}
