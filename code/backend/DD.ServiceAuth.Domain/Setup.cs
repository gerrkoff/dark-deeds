using DD.ServiceAuth.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DD.ServiceAuth.Domain;

public static class Setup
{
    public static void AddAuthServiceDomain(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITestService, TestService>();
    }
}
