using Domain.Models;

namespace Domain.Filters;

public class WarehouseFilter : AbstractProductFilter
{
    public override bool Apply(Product product, Filter filter)
    {
        if (filter.WarehouseId is not null && product.WarehouseId != filter.WarehouseId)
            return false;

        return base.Apply(product, filter);
    }
}