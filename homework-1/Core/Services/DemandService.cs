using Core.Abstractions;

namespace Core.Services;

public class DemandService : IDemandService
{
    private readonly ISalesPredictionService _salesPredictionService;
    private readonly IProductSalesRepository _productSalesRepository;

    public DemandService(ISalesPredictionService salesPredictionService, IProductSalesRepository productSalesRepository)
    {
        _salesPredictionService = salesPredictionService;
        _productSalesRepository = productSalesRepository;
    }

    public async Task<decimal> CalculateAsync(long productId, int daysAmount)
    {
        var salesPrediction = await _salesPredictionService.CalculateAsync(productId, daysAmount);
        var allSales = await _productSalesRepository.GetAllProductSalesAsync(productId);
        var lastSale = allSales.MaxBy(p => p.Date);
        var productAmount = lastSale.Stock - lastSale.Sales;
        
        return salesPrediction - productAmount;
    }
}