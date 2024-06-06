namespace HomeworkApp.Bll.Services.Interfaces;

public interface IRateLimiterService
{
    Task<bool> Allow(string ip, CancellationToken token);
}