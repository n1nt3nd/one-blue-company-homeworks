using Core.Models;

namespace Core.Abstractions;

public interface IProductSalesRepository
{
    Task<IEnumerable<ProductSale>> GetAllProductSalesAsync(long productId);
}