using DarkDeeds.AuthServiceApp.Data.Context;
using DarkDeeds.AuthServiceApp.Entities;
using DarkDeeds.Common.Misc;
using DarkDeeds.Communication.Interceptors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DarkDeeds.AuthServiceApp.App
{
    public static class StartupExtensions
    {
        public static void AddAuthServiceIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<UserEntity>(options =>
                {
#if DEBUG
                    options.Password.RequiredLength = 3;
#else
                    options.Password.RequiredLength = 8;
#endif
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DarkDeedsAuthContext>()
                .AddUserManager<UserManager<UserEntity>>();
            services.AddHttpContextAccessor();
        }
        
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

            var buildInfo = new BuildInfo(typeof(Startup),
                typeof(Contract.AuthService));
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