using DD.TelegramClient.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TelegramClient.Web
{
    public static class Extensions
    {
        public static void AddTelegramClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTelegramClientServices(configuration);
        }

        public static void MapTelegramClientCustomRoutes(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            endpoints.MapControllerRoute("bot", $"api/tlgm/bot/{configuration["Bot"]}",
                new { controller = "Bot", action = "Process" });
        }
    }
}
