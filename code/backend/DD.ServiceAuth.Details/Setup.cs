using System.Text;
using DD.ServiceAuth.Details.Web;
using DD.ServiceAuth.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DD.ServiceAuth.Details;

public static class Setup
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddAuthServiceWeb();
        services.AddAuthServiceDomain();
    }

    private static void AddAuthServiceWeb(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ModelsMapping));
    }

    public static void AddDdAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(options => configuration.GetSection("Auth").Bind(options));

        AuthSettings authSettings = configuration.GetSection("Auth").Get<AuthSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
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
