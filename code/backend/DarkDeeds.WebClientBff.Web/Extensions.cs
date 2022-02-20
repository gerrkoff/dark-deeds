using DarkDeeds.Communication;
using DarkDeeds.WebClientBff.Services;
using DarkDeeds.WebClientBff.UseCases;
using DarkDeeds.WebClientBff.Web.BackgroundServices;
using DarkDeeds.WebClientBff.Web.Hubs;
using DarkDeeds.WebClientBff.Web.Models;
using DarkDeeds.WebClientBff.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBff.Web
{
    public static class Extensions
    {
        public static void AddWebClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWebClientBffCommonServices();
            services.AddWebClientBffSockets();
            services.AddWebClientBffServices();
            services.AddWebClientBffUseCases();
            services.AddDarkDeedsAmpqSubscriber<TaskUpdatedSubscriber, TaskUpdatedDto>();
        }

        public static void MapWebClientCustomRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<TaskHub>("/ws/web/task");
        }

        private static void AddWebClientBffCommonServices(this IServiceCollection services)
        {
            services.AddTransient<NotifierService>();
            services.AddHttpContextAccessor();
        }

        private static void AddWebClientBffSockets(this IServiceCollection services)
        {
            services.AddHostedService<HubHeartbeat<TaskHub>>();
            services.AddTransient<IUserIdProvider, HubUserIdProvider>();
            services.AddSignalR()
                .AddHubOptions<TaskHub>(options => options.EnableDetailedErrors = true);
        }
    }
}
