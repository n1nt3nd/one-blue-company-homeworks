using Core.Services;
using Core.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection collection)
    {
        collection.AddScoped<IAdsService, AdsService>();
        collection.AddScoped<ISalesPredictionService, SalesPredictionService>();
        collection.AddScoped<IDemandService, DemandService>();

        collection.AddScoped<IDateProvider, DateProvider>();
        
        return collection;
    }
}