using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspNetCore.Identity.Mongo;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.Authentication;

namespace DD.ServiceAuth.Details;

public static class Setup
{
    public static void AddAuthService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthServiceDomain(configuration);
        services.AddAuthServiceData(configuration);
    }

    public static void UseDdAuthentication(this IApplicationBuilder app)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static void AddDdAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthSettings>()
            .Bind(configuration.GetSection("Auth"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var authSettings = configuration.GetSection("Auth").Get<AuthSettings>()
                           ?? throw new InvalidOperationException("Auth settings are not configured");

        var scopesSupported = configuration.GetSection("OAuth:ScopesSupported").Get<string[]>()
                              ?? throw new InvalidOperationException("OAuth scopes are not configured");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key)),
                    ValidateLifetime = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/ws", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                };
            })
            .AddMcp(options =>
            {
                // Only the /mcp endpoint challenges with this scheme (see MapMcp policy); token
                // validation itself is delegated to JwtBearer so /mcp accepts the same access token.
                options.ForwardAuthenticate = JwtBearerDefaults.AuthenticationScheme;
                options.ResourceMetadata = new ProtectedResourceMetadata
                {
                    ScopesSupported = scopesSupported,
                };
                options.Events.OnResourceMetadataRequest = context =>
                {
                    if (context.ResourceMetadata is not null)
                    {
                        var request = context.HttpContext.Request;
                        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                        context.ResourceMetadata.AuthorizationServers = [baseUrl];
                    }

                    return Task.CompletedTask;
                };
            });
    }

    private static void AddAuthServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("sharedDb");

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
            .AddMongoDbStores<UserEntity>(mongo =>
            {
                mongo.ConnectionString = connectionString;
                mongo.UsersCollection = "users";
                mongo.MigrationCollection = "dbMigrationsIdentity";
            })
            .AddUserManager<UserManager<UserEntity>>();
    }
}
