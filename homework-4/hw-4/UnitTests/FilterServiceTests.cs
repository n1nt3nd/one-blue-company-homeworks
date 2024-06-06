using AutoBogus;
using Domain.Enums;
using Domain.Models;
using Domain.Services;
using FluentAssertions;

namespace UnitTests;

public class FilterServiceTests
{
    private readonly IFilterService _filterService;

    public FilterServiceTests()
    {
        _filterService = new FilterService();
    }

    [Fact]
    void GetProductsWithFilter_AllFiltersAreNulls_ReturnAllProducts()
    {
        var expectedProducts = new AutoFaker<Product>().Generate(3);

        var filter = new Filter();

        var actualProducts = _filterService.GetProductsWithFilter(expectedProducts, filter);

        actualProducts.Should().BeEquivalentTo(expectedProducts);
    }
    
    [Fact]
    void GetProductsWithFilter_SomeFilter_ReturnPartOfProducts()
    {
        var warehouseId = 2;
        var expectedProducts = new AutoFaker<Product>()
            .RuleFor(f => f.ProductType, ProductType.Appliances)
            .RuleFor(f => f.WarehouseId, warehouseId)
            .Generate(10);

        var filter = new Filter()
        {
            ProductType = ProductType.Appliances,
            WarehouseId = warehouseId,
        };

        var actualProducts = _filterService.GetProductsWithFilter(expectedProducts, filter);

        actualProducts.Should().HaveCount(10);
    }
    
    [Fact]
    void GetProductsWithFilter_SomeFilter_ReturnEmptyCollection()
    {
        var warehouseId = 2;
        var expectedProducts = new AutoFaker<Product>()
            .RuleFor(f => f.ProductType, ProductType.Appliances)
            .RuleFor(f => f.WarehouseId, warehouseId)
            .Generate(10);

        var filter = new Filter()
        {
            ProductType = ProductType.Appliances,
            WarehouseId = 1,
        };

        var actualProducts = _filterService.GetProductsWithFilter(expectedProducts, filter);

        actualProducts.Should().BeEmpty();
    }
}