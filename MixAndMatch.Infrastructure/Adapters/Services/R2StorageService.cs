using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class R2StorageService(IOptions<R2Settings> _options) : IStorageService
{
    private AmazonS3Client CreateClient()
    {
        var s = _options.Value;
        var credentials = new BasicAWSCredentials(s.AccessKeyId, s.SecretKey);
        var config = new AmazonS3Config
        {
            ServiceURL   = $"https://{s.AccountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };
        return new AmazonS3Client(credentials, config);
    }

    public async Task<string> UploadAsync(string fileKey, Stream content, string contentType)
    {
        var s = _options.Value;
        using var client = CreateClient();
        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName           = s.BucketName,
            Key                  = fileKey,
            InputStream          = content,
            ContentType          = contentType,
            DisablePayloadSigning = true   // R2 no soporta STREAMING-AWS4-HMAC-SHA256-PAYLOAD-TRAILER
        });
        return $"{s.PublicUrl}/{fileKey}";
    }

    public async Task DeleteAsync(string fileKey)
    {
        var s = _options.Value;
        using var client = CreateClient();
        await client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = s.BucketName,
            Key        = fileKey
        });
    }

    public async Task DeleteByPrefixAsync(string prefix)
    {
        var s = _options.Value;
        using var client = CreateClient();

        string? continuationToken = null;
        do
        {
            var listResponse = await client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName        = s.BucketName,
                Prefix            = prefix,
                ContinuationToken = continuationToken
            });

            if (listResponse.S3Objects is null || listResponse.S3Objects.Count == 0) break;

            await client.DeleteObjectsAsync(new DeleteObjectsRequest
            {
                BucketName = s.BucketName,
                Objects    = listResponse.S3Objects
                                .Select(o => new KeyVersion { Key = o.Key })
                                .ToList()
            });

            continuationToken = listResponse.IsTruncated == true ? listResponse.NextContinuationToken : null;
        }
        while (continuationToken is not null);
    }

    public string GetFileKey(string url)
    {
        var prefix = $"{_options.Value.PublicUrl}/";
        return url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? url[prefix.Length..]
            : url;
    }
}
