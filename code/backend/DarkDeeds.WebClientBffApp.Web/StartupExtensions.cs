using DarkDeeds.Communication;
using DarkDeeds.WebClientBffApp.Communication;
using DarkDeeds.WebClientBffApp.Data;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using DarkDeeds.WebClientBffApp.Services;
using DarkDeeds.WebClientBffApp.Services.Dto;
using DarkDeeds.WebClientBffApp.UseCases;
using DarkDeeds.WebClientBffApp.Web.BackgroundServices;
using DarkDeeds.WebClientBffApp.Web.Hubs;
using DarkDeeds.WebClientBffApp.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.Web
{
    public static class StartupExtensions
    {
        public static void AddWebClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWebClientBffCommonServices();
            services.AddWebClientBffSockets();
            
            services.AddWebClientBffServices();
            services.AddWebClientBffUseCases();
            services.AddWebClientBffCommunications(configuration);
            services.AddWebClientBffData(configuration);
            services.AddWebClientBffCommonServices();
            services.AddWebClientBffSockets();
            
            services.AddDarkDeedsAmpqSubscriber<TaskUpdatedSubscriber, TaskUpdatedDto>();
        }
        
        public static void MapWebClientCustomRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<TaskHub>("/ws/web/task");
        }

        private static void AddWebClientBffCommonServices(this IServiceCollection services)
        {
            services.AddTransient<INotifierService, NotifierService>();
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