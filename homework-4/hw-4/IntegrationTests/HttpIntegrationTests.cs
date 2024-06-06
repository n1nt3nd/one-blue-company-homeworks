using System.Text;
using System.Web;
using Api;
using API;
using AutoBogus;
using FluentAssertions;
using IntegrationTests.Models;
using Moq;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Product = Domain.Models.Product;
using ProductType = Domain.Enums.ProductType;

namespace IntegrationTests;

public class HttpIntegrationTests : IClassFixture<MyCustomWebApplicationFactory<Startup>>
{
    private readonly MyCustomWebApplicationFactory<Startup> _webApplicationFactory;

    public HttpIntegrationTests(MyCustomWebApplicationFactory<Startup> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task Add_ShouldAddProduct()
    {
        // Arrange
        long expectedId = 1;
        var client = _webApplicationFactory.CreateClient();
        var addProductRequest = new AutoFaker<TestAddProductRequest>()
            .RuleFor(f => f.cost, p => p.Random.Double(0, 100))
            .RuleFor(f => f.weight, p => p.Random.Double(0, 100))
            .RuleFor(f => f.productType, p => p.Random.Int(0, 3))
            .RuleFor(f => f.creationDate, p => p.Date.Recent().ToString("yyyy-MM-ddTHH:mm:ss.0Z"))
            .RuleFor(f => f.warehouseId, p => p.Random.Int(1, 100))
            .Generate();

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(expectedId);
        
        // Act
        var json = JsonSerializer.Serialize(addProductRequest);
        var response = await client.PostAsync("/v1/products/add", new StringContent(json, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();

        
        // Assert
        var actual = AddProductResponse.Parser.ParseJson(responseString);

        response.EnsureSuccessStatusCode();
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expectedId);
    }

    [Fact]
    public async Task Get_ShouldReturnProduct()
    {
        // Arrange
        long productId = 1;
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.CreationDate, p => p.Date.Recent().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Int(1, 100))
            .Generate();

        var mappedProduct = _webApplicationFactory.Mapper.Map<API.Product>(expectedProduct);
        
        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        var client = _webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/v1/product/get?id={productId}");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        var actualProduct = API.Product.Parser.ParseJson(responseString);
        response.EnsureSuccessStatusCode();
        actualProduct.Should().Be(mappedProduct);
    }
    
    [Fact]
    public async Task UpdateCost_ShouldUpdateCostInProduct()
    {
        // Arrange
        long productId = 1;
        var client = _webApplicationFactory.CreateClient();
        var updateProductRequest = new AutoFaker<TestUpdateProductRequest>()
            .RuleFor(f => f.id, productId)
            .RuleFor(f => f.newCost, p => p.Random.Double(0, 100))
            .Generate();
        
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.Cost, p => p.Random.Double(0, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(0, 100))
            .RuleFor(f => f.CreationDate, p => p.Date.Recent().ToUniversalTime())
            .RuleFor(f => f.WarehouseId, p => p.Random.Int(1, 100))
            .Generate();
        
        var mappedProduct = _webApplicationFactory.Mapper.Map<API.Product>(expectedProduct);

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.UpdateCostByIdAsync(updateProductRequest.id, updateProductRequest.newCost))
            .ReturnsAsync(expectedProduct);
        
        // Act
        var json = JsonSerializer.Serialize(updateProductRequest);
        var response = await client.PutAsync("/v1/product/update", new StringContent(json, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        var actual = UpdateCostResponse.Parser.ParseJson(responseString);
        response.EnsureSuccessStatusCode();
        actual.Should().NotBeNull();
        actual.Product.Should().Be(mappedProduct);
    }
    
    [Fact]
    public async Task GetProductsWithFilter_ShouldReturnsSomeProducts()
    {
        // Arrange
        var warehouseId = 2;
        var expectedAmountItem = 5;
        var creationDate = DateTime.Today.ToUniversalTime();
        
        var products = new AutoFaker<Product>()
            .RuleFor(f => f.Cost, p => p.Random.Double(0, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(0, 100))
            .RuleFor(f => f.ProductType, ProductType.General)
            .RuleFor(f => f.WarehouseId, warehouseId)
            .RuleFor(f => f.CreationDate, p => p.Date.Past().ToUniversalTime())
            .Generate(10);


        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetAllAsync())
            .ReturnsAsync(products);

        var client = _webApplicationFactory.CreateClient();
        
        var queryParameters = new Dictionary<string, string>()
        {
            {"pages.pageNumber", "2"},
            {"pages.amountPerPage", "5"},
            {"filters.warehouseId", "2"},
            {"filters.productType.productType", "GENERAL"},
        };
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var dct in queryParameters)
        {
            query[dct.Key] = dct.Value;
        }
        
        var url = string.Join("?", "/v1/products/get", query.ToString());

        // Act
        var response = await client.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        var actualProducts = ProductsResponse.Parser.ParseJson(responseString);
        response.EnsureSuccessStatusCode();
        
        actualProducts.Should().NotBeNull();
        actualProducts.Product.Should().HaveCount(expectedAmountItem);
    }
}