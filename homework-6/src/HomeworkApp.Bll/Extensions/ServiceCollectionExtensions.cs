using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Providers;
using HomeworkApp.Utils.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeworkApp.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(
        this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskCommentService, TaskCommentService>();
        services.AddScoped<IRateLimiterService, RateLimiterService>();

        return services;
    }
}