using Core.Abstractions;

namespace Core.Services;

public class AdsService : IAdsService
{
    private readonly IProductSalesRepository _productSalesRepository;

    public AdsService(IProductSalesRepository productSalesRepository)
    {
        _productSalesRepository = productSalesRepository;
    }

    public async Task<decimal> CalculateAsync(long productId)
    {
        var sales = await _productSalesRepository.GetAllProductSalesAsync(productId);

        if (!sales.Any())
            return 0;

        return (decimal)sales.Where(p => p.Stock != 0).Average(p => p.Sales);
    }
}