using Core.Models;

namespace DataAccess.Parsers;

public interface IProductSalesParser
{
    Task<IEnumerable<ProductSale>> ParseAsync(string filePath);
}