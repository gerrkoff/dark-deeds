using DD.ServiceAuth.Domain.Services;
using DD.Shared.Details.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.ServiceAuth.Domain;

public static class Setup
{
    public static void AddAuthServiceDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OAuthSettings>()
            .Bind(configuration.GetSection("OAuth"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITestService, TestService>();
        services.AddTransient<IClaimsService, ClaimsService>();
        services.AddTransient<IPkceService, PkceService>();
        services.AddScoped<IAuthCodeService, AuthCodeService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
    }
}
