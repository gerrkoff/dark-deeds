using DD.MobileClient.Details.Data;
using DD.MobileClient.Details.Infrastructure;
using DD.MobileClient.Domain;
using DD.MobileClient.Domain.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DD.MobileClient.Details;

public static class Setup
{
    public static void AddMobileClient(this IServiceCollection services)
    {
        services.AddMobileClientInfrastructure();
        services.AddMobileClientDomain();
        services.AddMobileClientData();
    }

    private static void AddMobileClientInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskServiceApp, TaskServiceApp>();
        services.AddAutoMapper(typeof(ModelsMapping));
    }

    private static void AddMobileClientData(this IServiceCollection services)
    {
        services.AddScoped<MobileUserRepository>();
        services.AddScoped<IMobileUserRepository, MobileUserRepository>();
    }
}
