using AutoBogus;
using Domain.Abstractions;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Exceptions;
using Infrastructure.Repositories;

namespace UnitTests;

public class ProductRepositoryTests
{
    private readonly IProductRepository _productRepository;

    public ProductRepositoryTests()
    {
        _productRepository = new ProductRepository();
    }


    [Fact]
    public async Task AddAsync_ProductExistInRepository_ShouldReturnProductFromRepository()
    {
        // Arrange
        var product = new AutoFaker<Product>().Generate();

        // Act
        await _productRepository.AddAsync(product);

        // Assert
        var products = await _productRepository.GetAllAsync();
        products.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_ProductExistInRepository_ShouldReturnProductFromRepository()
    {
        // Arrange
        var expectedProduct = new AutoFaker<Product>().Generate();

        // Act
        var productId = await _productRepository.AddAsync(expectedProduct);
        var actualProduct = await _productRepository.GetByIdAsync(productId);

        // Assert
        actualProduct.Should().Be(expectedProduct);
    }

    [Fact]
    public async Task GetById_ProductNotExistInRepository_ShouldReturnRepositoryException()
    {
        // Arrange
        const int productId = 1;

        // Act
        var act = () => _productRepository.GetByIdAsync(productId);

        // Assert
        await act.Should().ThrowAsync<RepositoryException>();
    }

    [Fact]
    public async Task UpdateCost_ProductExistInRepository_ShouldReturnProductWithNewCost()
    {
        // Arrange
        var product = new AutoFaker<Product>().Generate();
        var newCost = 1.0;

        // Act
        var productId = await _productRepository.AddAsync(product);
        var actualProduct = await _productRepository.UpdateCostByIdAsync(productId, newCost);

        var expectedProduct = product;
        expectedProduct.Cost = newCost;

        // Assert
        actualProduct.Should().Be(expectedProduct);
    }

    [Fact]
    public async Task UpdateCost_ProductNotExistInRepository_ShouldReturnRepositoryException()
    {
        // Arrange
        const int productId = 1;
        const double cost = 1.0;

        // Act
        var act = () => _productRepository.UpdateCostByIdAsync(productId, cost);

        // Assert
        await act.Should().ThrowAsync<RepositoryException>();
    }


}