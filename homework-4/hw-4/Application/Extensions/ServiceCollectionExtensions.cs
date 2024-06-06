using Application.Services;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<IFilterService, FilterService>();
        
        return serviceCollection;
    }
}