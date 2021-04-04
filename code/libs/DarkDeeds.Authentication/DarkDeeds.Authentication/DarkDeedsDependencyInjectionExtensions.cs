using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.Authentication.Models;
using DarkDeeds.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DarkDeedsDependencyInjectionExtensions
    {
        private const string AuthSection = "Auth";

        public static void AddDarkDeedsAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<AuthSettings>(options => configuration.GetSection(AuthSection).Bind(options));
            
            AuthSettings authSettings = configuration.GetSection(AuthSection).Get<AuthSettings>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = TokenValidationParams.Get(authSettings);
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
