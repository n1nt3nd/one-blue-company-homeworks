using Api.Mapping;
using AutoMapper;
using Domain.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace IntegrationTests;

public class MyCustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    public Mock<IProductRepository> ProductRepositoryFake { get; }
    public IMapper Mapper { get; }

    public MyCustomWebApplicationFactory()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
            cfg.AddProfile<ProductTypeProfile>();
            cfg.AddProfile<DateTimeProfile>();
            cfg.AddProfile<FilterProfile>();
        });

        ProductRepositoryFake = new();
        Mapper = configuration.CreateMapper();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Replace(new ServiceDescriptor(typeof(IProductRepository), ProductRepositoryFake.Object));
        });
    }
}