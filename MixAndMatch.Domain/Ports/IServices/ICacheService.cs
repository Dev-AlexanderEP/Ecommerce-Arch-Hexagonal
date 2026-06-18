namespace MixAndMatch.Domain.Ports.IServices;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan expiration);
    Task<string?> GetAsync(string key);
    Task DeleteAsync(string key);
}
