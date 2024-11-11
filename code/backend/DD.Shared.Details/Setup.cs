using DD.ServiceTask.Domain.Infrastructure;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Controllers;
using DD.Shared.Details.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Shared.Details;

public static class Setup
{
    public static void AddSharedDetails(this IServiceCollection services)
    {
        services.AddScoped<ITaskServiceApp, TaskServiceApp>();
        services.AddScoped<INotifierService, TaskServiceNotifier>();
        services.AddSingleton<ITaskServiceNotifierChannelProvider, TaskServiceNotifierChannelProvider>();
        services.AddHostedService<TaskServiceNotifierBackgroundService>();
        services.AddMemoryCache();
        services.AddScoped<ICacheProvider, CacheProvider>();
        services.AddScoped<ITaskPrinter, TaskPrinter>();
        services.AddTransient<IAuthTokenConverter, AuthTokenConverter>();
        services.AddScoped<TestAttribute>();
        services.AddScoped<IUserAuth, UserAuth>();
        services.AddScoped<IValidator, Validator>();
    }
}
