using Core.Abstractions;
using Core.Exceptions;
using Core.Models;
using DataAccess.Parsers;

namespace DataAccess.Repositories;

public class ProductSalesRepository : IProductSalesRepository
{
    private readonly string _filePath;
    private readonly IProductSalesParser _productSalesParser;

    public ProductSalesRepository(IProductSalesParser productSalesParser, string? filePath)
    {
        _productSalesParser = productSalesParser;

        _filePath = filePath ??
                    throw new IncorrectDataException("The path to the sales_history.txt file is not specified");
    }

    public async Task<IEnumerable<ProductSale>> GetAllProductSalesAsync(long productId)
    {
        var productSales = await _productSalesParser.ParseAsync(_filePath);
        var result = productSales.Where(p => p.Id == productId);

        return result;
    }
}