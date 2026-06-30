namespace MixAndMatch.Domain.Ports.IServices;

public interface IStorageService
{
    Task<string> UploadAsync(string fileKey, Stream content, string contentType);
    Task DeleteAsync(string fileKey);
    Task DeleteByPrefixAsync(string prefix);
    string GetFileKey(string url);
}
