using DD.Clients.Details.MobileClient.Data;
using DD.Clients.Details.TelegramClient.Data;
using DD.Clients.Details.WebClientBff.Data;
using DD.MobileClient.Domain;
using DD.MobileClient.Domain.Infrastructure;
using DD.TelegramClient.Domain;
using DD.TelegramClient.Domain.Infrastructure;
using DD.WebClientBff.Domain;
using DD.WebClientBff.Domain.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Clients.Details;

public static class Setup
{
    public static void AddClients(this IServiceCollection services, IConfiguration configuration)
    {
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
