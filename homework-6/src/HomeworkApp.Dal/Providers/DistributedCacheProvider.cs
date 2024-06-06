using HomeworkApp.Utils.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Dal.Providers;

public class DistributedCacheProvider : IDistributedCacheProvider
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheProvider(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken token)
    {
        return await _distributedCache.GetStringAsync(key, token);
    }

    public async Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token)
    {
        await _distributedCache.SetStringAsync(key, value, options: options, token: token);
    }

    public async Task SetStringAsync(string key, string value, CancellationToken token)
    {
        await _distributedCache.SetStringAsync(key, value, token: token);
    }
}