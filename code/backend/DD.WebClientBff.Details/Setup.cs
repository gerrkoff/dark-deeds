using DD.WebClientBff.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DD.WebClientBff.Details;

public static class Setup
{
    public static void AddWebClientBff(this IServiceCollection services)
    {
        services.AddWebClientBffDomain();
    }
}
