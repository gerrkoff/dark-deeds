using DD.MobileClient.Domain;
using DD.MobileClient.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Common;
using DD.Shared.Details.MobileClient.Data;
using DD.Shared.Details.TelegramClient.Data;
using DD.Shared.Details.WebClientBff.Data;
using DD.TelegramClient.Domain;
using DD.TelegramClient.Domain.Infrastructure;
using DD.WebClientBff.Domain;
using DD.WebClientBff.Domain.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Shared.Details;

public static class Setup
{
    public static void AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddClientInfrastructure();

        services.AddTelegramClientDomain(configuration);
        services.AddTelegramClientData();

        services.AddMobileClientDomain();
        services.AddMobileClientData();

        services.AddWebClientBffDomain();
        services.AddWebClientBffData();
    }

    public static void MapClientsCustomRoutes(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
    {
        endpoints.MapControllerRoute(
            "bot",
            $"api/tlgm/bot/{configuration["Bot"]}",
            new { controller = "Bot", action = "Process" });
    }

    private static void AddClientInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskServiceApp, TaskServiceApp>();
        services.AddScoped<INotifierService, TaskServiceNotifier>();
        services.AddSingleton<ITaskServiceNotifierChannelProvider, TaskServiceNotifierChannelProvider>();
        services.AddHostedService<TaskServiceNotifierBackgroundService>();
        services.AddMemoryCache();
        services.AddScoped<ICacheProvider, CacheProvider>();
        services.AddScoped<ITaskPrinter, TaskPrinter>();
        services.AddTransient<IAuthTokenConverter, AuthTokenConverter>();
    }

    private static void AddTelegramClientData(this IServiceCollection services)
    {
        services.AddScoped<TelegramUserRepository>();
        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
    }

    private static void AddMobileClientData(this IServiceCollection services)
    {
        services.AddScoped<MobileUserRepository>();
        services.AddScoped<IMobileUserRepository, MobileUserRepository>();
    }

    private static void AddWebClientBffData(this IServiceCollection services)
    {
        services.AddScoped<UserSettingsRepository>();
        services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
    }
}
