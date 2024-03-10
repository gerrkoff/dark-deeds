using DD.WebClientBff.Details.Data;
using DD.WebClientBff.Domain;
using DD.WebClientBff.Domain.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DD.WebClientBff.Details;

public static class Setup
{
    public static void AddWebClientBff(this IServiceCollection services)
    {
        services.AddWebClientBffDomain();
        services.AddWebClientBffData();
    }

    private static void AddWebClientBffData(this IServiceCollection services)
    {
        services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
    }
}
