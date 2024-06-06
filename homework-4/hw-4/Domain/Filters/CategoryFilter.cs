using Domain.Models;

namespace Domain.Filters;

public class CategoryFilter : AbstractProductFilter
{
    public override bool Apply(Product product, Filter filter)
    {
        if (filter.ProductType is not null && product.ProductType != filter.ProductType)
            return false;

        return base.Apply(product, filter);
    }
}