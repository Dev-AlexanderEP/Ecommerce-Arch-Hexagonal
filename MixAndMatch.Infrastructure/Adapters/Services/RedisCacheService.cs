using MixAndMatch.Domain.Ports.IServices;
using StackExchange.Redis;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class RedisCacheService(IConnectionMultiplexer _redis) : ICacheService
{
    private readonly IDatabase _db = _redis.GetDatabase();

    public async Task SetAsync(string key, string value, TimeSpan expiration)
        => await _db.StringSetAsync(key, value, expiration);

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task DeleteAsync(string key)
        => await _db.KeyDeleteAsync(key);
}
