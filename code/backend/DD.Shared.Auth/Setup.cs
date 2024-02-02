using Microsoft.Extensions.DependencyInjection;

namespace DD.Shared.Auth;

public static class Setup
{
    public static void AddSharedAuth(this IServiceCollection services)
    {
        services.AddTransient<IAuthTokenConverter, AuthTokenConverter>();
    }
}
