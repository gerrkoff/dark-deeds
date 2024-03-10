using DD.WebClientBff.Domain.Mapping;
using DD.WebClientBff.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DD.WebClientBff.Domain;

public static class Setup
{
    public static void AddWebClientBffDomain(this IServiceCollection services)
    {
        services.AddScoped<IUserSettingsService, UserSettingsService>();
        services.AddAutoMapper(typeof(ModelsMapping));
    }
}
