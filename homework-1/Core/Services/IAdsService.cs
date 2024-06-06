namespace Core.Services;

public interface IAdsService
{
    Task<decimal> CalculateAsync(long productId);
}