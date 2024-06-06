using Application.Dto;
using AutoMapper;
using Domain.Abstractions;
using Domain.Models;
using Domain.Services;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IFilterService _filterService;

    public ProductService(IProductRepository productRepository, IMapper mapper, IFilterService filterService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _filterService = filterService;
    }

    public async Task<long> AddAsync(ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);

        var productId = await _productRepository.AddAsync(product);

        return productId;
    }

    public async Task<Product> GetByIdAsync(long productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        return product;
    }

    public async Task<Product> UpdateCostByIdAsync(long productId, double newCost)
    {
        var product = await _productRepository.UpdateCostByIdAsync(productId, newCost);

        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsWithFilter(Filter filter)
    {
        var products = await _productRepository.GetAllAsync();
        var filteredProducts = _filterService.GetProductsWithFilter(products, filter);

        return filteredProducts;
    }

    public Task<IEnumerable<Product>> GetPage(IEnumerable<Product> products, int pageNumber, int amountPerPage)
    {
        var needSkip = (pageNumber - 1) * amountPerPage;
        var needProducts = products.Skip(needSkip).Take(amountPerPage);
        
        return Task.FromResult(needProducts);
    }
}