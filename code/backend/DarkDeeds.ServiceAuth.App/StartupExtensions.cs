using DarkDeeds.Common.Misc;
using DarkDeeds.Communication.Interceptors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DarkDeeds.ServiceAuth.App
{
    static class StartupExtensions
    {   
        public static void AddAuthServiceApi(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionInterceptor>();
            });
            services.AddGrpcReflection();

            services.AddControllers(options =>
            {
                var authRequired = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(authRequired));
            });

            var buildInfo = new BuildInfo(typeof(Startup), typeof(Contract.AuthService));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DarkDeeds.AuthService",
                    Version = $"{buildInfo.AppVersion} / {buildInfo.ContractVersion}",
                    Description = "Check gRPC contract",
                });
            });
        }
    }
}