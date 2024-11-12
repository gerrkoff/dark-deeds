using DD.ServiceTask.Domain.Infrastructure;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Controllers;
using DD.Shared.Details.Data;
using DD.Shared.Details.Services;
using Microsoft.Extensions.Configuration;
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
        services.AddScoped<TestAttribute>();
        services.AddScoped<IUserAuth, UserAuth>();
        services.AddScoped<IValidator, Validator>();
    }

    public static void AddSharedData(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("sharedDb")
                                    ?? throw new InvalidOperationException("Connection string for sharedDb is not found");
        services.AddSingleton<IMigratorMongoDbContext>(_ => new MongoDbContext(mongoConnectionString));
        services.AddSingleton<IMongoDbContext>(sp => sp.GetRequiredService<IMigratorMongoDbContext>());
    }
}
