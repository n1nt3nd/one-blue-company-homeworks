using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class RateLimiterRepository : RedisRepository, IRateLimiterRepository
{
    protected override string KeyPrefix => "rate_limiter";

    public RateLimiterRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<string> Get(string ip, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var connection = await GetConnection();
        
        var key = GetKey(ip);
        var value = await connection.StringGetAsync(key);
        
        return value.ToString();
    }

    public async Task Set(string ip, string json, TimeSpan? expiry, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var connection = await GetConnection();
        var key = GetKey(ip);
        
        await connection.StringSetAsync(key, json, expiry);
    }
}