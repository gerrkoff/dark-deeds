using DD.MobileClient.Domain.Services;
using DD.MobileClient.Domain.Subscriptions;
using DD.Shared.Details.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DD.MobileClient.Domain;

public static class Setup
{
    public static void AddMobileClientDomain(this IServiceCollection services)
    {
        services.AddScoped<IWatchService, WatchService>();
        services.AddScoped<IWatchPayloadController, WatchPayloadController>();
        services.AddScoped<ITaskServiceSubscriber, TaskServiceSubscriber>();
    }
}
