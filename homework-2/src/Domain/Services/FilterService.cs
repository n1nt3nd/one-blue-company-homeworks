using Domain.Filters;
using Domain.Models;

namespace Domain.Services;

public class FilterService : IFilterService
{
    private readonly IProductFilter _filter;

    public FilterService()
    {
        _filter = new CategoryFilter();
        _filter.AddNext(new WarehouseFilter());
        _filter.AddNext(new CreationDateFilter());
    }

    public IEnumerable<Product> GetProductsWithFilter(IEnumerable<Product> products, Filter filter)
    {
        var filteredProducts = products.Where(product => _filter.Apply(product, filter));

        return filteredProducts;
    }
}