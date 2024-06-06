using Api;
using API;
using AutoBogus;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Moq;
using Product = Domain.Models.Product;
using ProductType = Domain.Enums.ProductType;

namespace IntegrationTests;

public class GrpcIntegrationTests : IClassFixture<MyCustomWebApplicationFactory<Startup>>
{
    private readonly MyCustomWebApplicationFactory<Startup> _webApplicationFactory;

    public GrpcIntegrationTests(MyCustomWebApplicationFactory<Startup> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task Get_SomeProductId_ShouldReturnProduct()
    {
        // Arrange
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        var grpcClient = new ProductService.ProductServiceClient(channel);

        var productId = 1;
        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, productId)
            .RuleFor(f => f.Cost, f => f.Random.Double(0, 100))
            .RuleFor(f => f.Weight, f => f.Random.Double(0, 100))
            .RuleFor(f => f.CreationDate, f => f.Date.Past().ToUniversalTime())
            .Generate();

        var mappedProduct = _webApplicationFactory.Mapper.Map<API.Product>(expectedProduct);
        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        var getProductRequest = new GetProductRequest()
        {
            Id = 1
        };

        // Act
        var actualProduct = await grpcClient.GetByIdAsync(getProductRequest);

        // Assert
        actualProduct.Should().NotBeNull();
        actualProduct.Should().Be(mappedProduct);
    }

    [Fact]
    public async Task Add_SomeProduct()
    {
        // Arrange
        long expectedId = 1;
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        var addProductRequest = new AutoFaker<AddProductRequest>()
            .RuleFor(f => f.Cost, p => p.Random.Double(0, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(0, 100))
            .RuleFor(f => f.CreationDate, p => p.Date.Past().ToUniversalTime().ToTimestamp())
            .Generate();

        var grpcClient = new ProductService.ProductServiceClient(channel);

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(expectedId);

        // Act
        var response = await grpcClient.AddAsync(addProductRequest);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(expectedId);
    }

    [Fact]
    public async Task UpdateCost_ShouldUpdateCost()
    {
        // Arrange
        long expectedId = 1;
        var newCost = 0;
        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        var updateCostRequest = new AutoFaker<UpdateCostRequest>()
            .RuleFor(f => f.Id, expectedId)
            .RuleFor(f => f.NewCost, newCost)
            .Generate();

        var expectedProduct = new AutoFaker<Product>()
            .RuleFor(f => f.Id, expectedId)
            .RuleFor(f => f.Cost, newCost)
            .RuleFor(f => f.Weight, f => f.Random.Double(0, 100))
            .RuleFor(f => f.CreationDate, f => f.Date.Past().ToUniversalTime())
            .Generate();

        var mappedProduct = _webApplicationFactory.Mapper.Map<API.Product>(expectedProduct);
        
        var grpcClient = new ProductService.ProductServiceClient(channel);

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.UpdateCostByIdAsync(updateCostRequest.Id, updateCostRequest.NewCost))
            .ReturnsAsync(expectedProduct);

        // Act
        var response = await grpcClient.UpdateCostByIdAsync(updateCostRequest);

        // Assert
        response.Should().NotBeNull();
        response.Product.Should().Be(mappedProduct);
    }

    [Fact]
    public async Task GetProductsWithFilter_WithSomeFilterAndPagination_ShouldReturnSomeProducts()
    {
        // Arrange
        var warehouseId = 2;
        var expectedAmountItem = 3;
        var creationDate = DateTime.Today.ToUniversalTime();

        var webAppClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(webAppClient.BaseAddress, new GrpcChannelOptions()
        {
            HttpClient = webAppClient
        });

        var products = new AutoFaker<Product>()
            .RuleFor(f => f.Cost, p => p.Random.Double(0, 100))
            .RuleFor(f => f.Weight, p => p.Random.Double(0, 100))
            .RuleFor(f => f.ProductType, ProductType.HouseholdChemicals)
            .RuleFor(f => f.CreationDate, creationDate)
            .RuleFor(f => f.WarehouseId, warehouseId)
            .Generate(10);

        var getProductsRequest = new GetProductsRequest()
        {
            Filters = new Filter()
            {
                WarehouseId = warehouseId,
                CreationDate = creationDate.ToTimestamp(),
                ProductType = new NullableProductType()
                {
                    ProductType = API.ProductType.HouseholdChemicals
                }
            },
            Pages = new Pagination()
            {
                AmountPerPage = 3,
                PageNumber = 3
            }
        };

        var grpcClient = new ProductService.ProductServiceClient(channel);

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var response = await grpcClient.GetProductsAsync(getProductsRequest);

        // Assert
        response.Should().NotBeNull();
        response.Product.Should().HaveCount(expectedAmountItem);
    }
}