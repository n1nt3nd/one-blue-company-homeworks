using Application.Dto;
using Application.Services;
using AutoBogus;
using AutoMapper;
using Domain.Abstractions;
using Domain.Models;
using Domain.Services;
using FluentAssertions;
using Infrastructure.Exceptions;
using Moq;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryFake = new(MockBehavior.Strict);
    private readonly Mock<IMapper> _mapperFake = new(MockBehavior.Strict);
    private readonly Mock<IFilterService> _filterServiceFake = new(MockBehavior.Strict);

    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productService = new ProductService(_productRepositoryFake.Object, _mapperFake.Object, _filterServiceFake.Object);
    }

    [Fact]
    public async Task GetById_ProductExistInRepository_ShouldReturnProductFromRepository()
    {
        var expectedProduct = new AutoFaker<Product>().Generate();
        var productId = expectedProduct.Id;

        _productRepositoryFake
            .Setup(f => f.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        Product actualProduct = await _productService.GetByIdAsync(productId);

        actualProduct.Should().Be(expectedProduct);
    }


    [Fact]
    public async Task UpdateCost_ProductExistInRepository_ShouldReturnProductWithNewCost()
    {
        var expectedProduct = new AutoFaker<Product>().Generate();
        var productId = expectedProduct.Id;
        var cost = expectedProduct.Cost;

        _productRepositoryFake
            .Setup(f => f.UpdateCostByIdAsync(productId, cost))
            .ReturnsAsync(expectedProduct);

        var actualProduct = await _productService.UpdateCostByIdAsync(productId, cost);

        actualProduct.Should().Be(expectedProduct);
    }


    [Fact]
    public async Task GetPage_ExistingElements_ShouldReturnElements()
    {
        const int countElements = 15;
        const int pageNumber = 3;
        const int amountPerPage = 5;
        var products = new AutoFaker<Product>().Generate(countElements);
        var expectedProducts = products.TakeLast(5);

        var actualProducts = await _productService.GetPage(products, pageNumber, amountPerPage);

        actualProducts.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetPage_NotExistingElements_ShouldReturnEmptyCollection()
    {
        const int countElements = 100;
        const int pageNumber = 100;
        const int amountPerPage = 5;
        var products = new AutoFaker<Product>().Generate(countElements);

        var actualProducts = await _productService.GetPage(products, pageNumber, amountPerPage);

        actualProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task Add_SomeProduct_ShouldCallAddInRepository()
    {
        var productDto = new AutoFaker<ProductDto>().Generate();
        var product = new AutoFaker<Product>().Generate();
        var expectedProductId = 0;
        _mapperFake.Setup(f => f.Map<Product>(productDto)).Returns(product);
        _productRepositoryFake.Setup(f => f.AddAsync(product)).ReturnsAsync(expectedProductId);

        var actualProductId = await _productService.AddAsync(productDto);

        _productRepositoryFake.Verify(f => f.AddAsync(product), Times.Once);
        actualProductId.Should().Be(expectedProductId);
    }
}