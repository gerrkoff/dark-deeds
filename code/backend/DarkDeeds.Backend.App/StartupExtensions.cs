using DarkDeeds.Common.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DarkDeeds.Backend.App
{
    public static class StartupExtensions
    {
        public static void AddBackendApi(this IServiceCollection services)
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
                    Title = "DarkDeeds.Backend",
                    Version = buildInfo.AppVersion
                });
            });
        }
    }
}