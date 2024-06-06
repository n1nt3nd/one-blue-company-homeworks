using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Utils.Helpers;

public interface IDistributedCacheProvider
{
    public Task<string?> GetStringAsync(string key, CancellationToken token);

    public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token);

    public Task SetStringAsync(string key, string value, CancellationToken token);
}