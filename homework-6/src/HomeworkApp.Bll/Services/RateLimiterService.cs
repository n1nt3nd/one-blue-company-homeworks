using System.Text.Json;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Utils.Helpers;

namespace HomeworkApp.Bll.Services;

public class RateLimiterService : IRateLimiterService
{
    private const int requestQuantity = 3;
    private const int timeUnit = 60;

    private readonly IRateLimiterRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RateLimiterService(
        IRateLimiterRepository rateLimiterRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _repository = rateLimiterRepository;
    }

    public async Task<bool> Allow(string ip, CancellationToken token)
    {
        var cachedTask = await _repository.Get(ip, token);

        var rateLimiterModel = new RateLimiterModel();
        if (string.IsNullOrEmpty(cachedTask))
        {
            rateLimiterModel.Bucket = requestQuantity;
            rateLimiterModel.LastCheck = _dateTimeProvider.GetUtcNow();
        }
        else rateLimiterModel = JsonSerializer.Deserialize<RateLimiterModel>(cachedTask);

        var timePassed = (_dateTimeProvider.GetUtcNow() - rateLimiterModel.LastCheck).TotalSeconds;
        rateLimiterModel.Bucket = (int) Math.Min(rateLimiterModel.Bucket + timePassed * ((double) requestQuantity / timeUnit), requestQuantity);

        if (rateLimiterModel.Bucket < 1)
            return false;

        rateLimiterModel.DecrementBucket();
        rateLimiterModel.LastCheck = _dateTimeProvider.GetUtcNow();

        var rateLimiterJson = JsonSerializer.Serialize(rateLimiterModel);

        await _repository.Set(
            ip,
            rateLimiterJson,
            TimeSpan.FromSeconds(timeUnit),
            token
        );

        return true;
    }
}