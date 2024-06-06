namespace Core.Services;

public interface ISalesPredictionService
{
    Task<decimal> CalculateAsync(long productId, int daysAmount);
}