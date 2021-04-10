using DarkDeeds.Common.Misc;
using DarkDeeds.WebClientBffApp.App.Hubs;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DarkDeeds.WebClientBffApp.App
{
    public static class StartupExtensions
    {
        public static void AddWebClientBffCommonServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskHubService, TaskHubService>();
            services.AddHttpContextAccessor();
        }

        public static void AddWebClientBffApi(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                var authRequired = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(authRequired));
            });
            
            var buildInfo = new BuildInfo(typeof(Startup), null);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DarkDeeds.WebClientBff",
                    Version = $"v{buildInfo.AppVersion}",
                });
            });
        }

        public static void AddWebClientBffSockets(this IServiceCollection services)
        {
            services.AddHostedService<HubHeartbeat<TaskHub>>();
            services.AddSignalR()
                .AddHubOptions<TaskHub>(options => options.EnableDetailedErrors = true);
        }
    }
}