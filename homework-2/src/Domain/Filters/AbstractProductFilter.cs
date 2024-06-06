using Domain.Models;

namespace Domain.Filters;

public abstract class AbstractProductFilter : IProductFilter
{
    private IProductFilter? _next = null;
    
    public IProductFilter AddNext(IProductFilter productFilter)
    {
        if (_next is not null)
            return _next.AddNext(productFilter);

        _next = productFilter;

        return _next;
    }

    public virtual bool Apply(Product product, Filter filter)
    {
        if (_next is not null)
            return _next.Apply(product, filter);
        
        return true;
    }
}