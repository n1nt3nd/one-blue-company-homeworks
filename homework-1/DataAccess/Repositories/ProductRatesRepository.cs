using Core.Abstractions;
using Core.Exceptions;
using Core.Models;
using DataAccess.Parsers;

namespace DataAccess.Repositories;

public class ProductRatesRepository : IProductRatesRepository
{
    private readonly string _filePath;
    private readonly IProductRatesParser _productRatesParser;

    public ProductRatesRepository(IProductRatesParser productRatesParser, string? filePath)
    {
        _productRatesParser = productRatesParser;
        
        _filePath = filePath ??
                    throw new IncorrectDataException("The path to the product_rates.txt file is not specified");
    }

    public async Task<ProductRate?> GetProductRateAsync(long productId, int month)
    {
        var productSales = await _productRatesParser.ParseAsync(_filePath);
        return productSales.FirstOrDefault(p => p.Id == productId && p.Month == month);
    }
}