using DD.TelegramClient.Details.Infrastructure;
using DD.TelegramClient.Domain;
using DD.TelegramClient.Domain.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.TelegramClient.Details;

public static class Setup
{
    public static void AddTelegramClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTelegramClientInfrastructure();
        services.AddTelegramClientDomain(configuration);

    }

    private static void AddTelegramClientInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskServiceApp, TaskServiceApp>();
        services.AddAutoMapper(typeof(ModelsMapping));
    }

    public static void MapTelegramClientCustomRoutes(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
    {
        endpoints.MapControllerRoute("bot", $"api/tlgm/bot/{configuration["Bot"]}",
            new { controller = "Bot", action = "Process" });
    }
}
