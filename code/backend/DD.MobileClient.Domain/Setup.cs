using DD.MobileClient.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DD.MobileClient.Domain;

public static class Setup
{
    public static void AddMobileClientDomain(this IServiceCollection services)
    {
        services.AddScoped<IWatchService, WatchService>();
    }
}
