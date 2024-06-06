using Core.Models;

namespace Core.Abstractions;

public interface IProductRatesRepository
{
    Task<ProductRate?> GetProductRateAsync(long productId, int month);
}