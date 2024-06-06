using System.Collections.Concurrent;
using Domain.Abstractions;
using Domain.Models;
using Infrastructure.Exceptions;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<long, Product> _store = new();
    private volatile int _amountAdded = 0;
    private readonly object _lock = new();

    public Task<long> AddAsync(Product product)
    {
        long productId;
        
        lock (_lock)
        {
            productId = _amountAdded++;
            product.Id = productId;
        }

        var added = _store.TryAdd(productId, product);

        if (!added)
            throw new RepositoryException("No value was added");

        return Task.FromResult(productId);
    }

    public Task<Product> GetByIdAsync(long productId)
    {
        if (!_store.TryGetValue(productId, out var product))
            throw new RepositoryException("No value found");

        return Task.FromResult(product);
    }

    public async Task<Product> UpdateCostByIdAsync(long productId, double newCost)
    {
        var product = await GetByIdAsync(productId);

        lock (_lock)
        {
            product.Cost = newCost;
        }

        return product;
    }
    
    public Task<List<Product>> GetAllAsync()
    {
        var products = _store.Values.ToList();

        return Task.FromResult(products);
    }
}