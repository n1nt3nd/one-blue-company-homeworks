using Core.Abstractions;
using DataAccess.Configuration;
using DataAccess.Parsers;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureDataAccess(
        this IServiceCollection collection,
        FilesConfiguration configuration)
    {
        collection.AddScoped<IProductRatesParser, ProductRatesParser>();
        collection.AddScoped<IProductSalesParser, ProductSalesParser>();

        collection.AddScoped<IProductRatesRepository, ProductRatesRepository>(x =>
            new ProductRatesRepository(x.GetRequiredService<IProductRatesParser>(),
                configuration.FilePathProductRates));

        collection.AddScoped<IProductSalesRepository, ProductSalesRepository>(x =>
            new ProductSalesRepository(x.GetRequiredService<IProductSalesParser>(),
                configuration.FilePathSalesHistory));

        return collection;
    }
}