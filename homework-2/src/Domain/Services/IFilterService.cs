using Domain.Models;

namespace Domain.Services;

public interface IFilterService
{
    IEnumerable<Product> GetProductsWithFilter(IEnumerable<Product> products, Filter filter);
}