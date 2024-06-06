using Core.Models;

namespace DataAccess.Parsers;

public interface IProductRatesParser
{
    Task<IEnumerable<ProductRate>> ParseAsync(string filePath);
}