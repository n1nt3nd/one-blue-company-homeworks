namespace Core.Services;

public interface IDemandService
{
    Task<decimal> CalculateAsync(long productId, int daysAmount);
}