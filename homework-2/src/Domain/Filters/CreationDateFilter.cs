using Domain.Models;

namespace Domain.Filters;

public class CreationDateFilter : AbstractProductFilter
{
    public override bool Apply(Product product, Filter filter)
    {
        if (filter.CreationDate is not null && product.CreationDate != filter.CreationDate)
            return false;

        return base.Apply(product, filter);
    }
}