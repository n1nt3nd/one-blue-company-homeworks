using Domain.Models;

namespace Domain.Filters;

public interface IProductFilter
{
    IProductFilter AddNext(IProductFilter productFilter);
    bool Apply(Product product, Filter filter);
}