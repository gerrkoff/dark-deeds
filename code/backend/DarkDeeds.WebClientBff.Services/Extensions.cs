using DarkDeeds.WebClientBff.Services.Mapping;
using DarkDeeds.WebClientBff.Services.Services.Implementation;
using DarkDeeds.WebClientBff.Services.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBff.Services
{
    public static class Extensions
    {
        public static void AddWebClientBffServices(this IServiceCollection services)
        {
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddAutoMapper(typeof(ModelsMapping));
        }
    }
}
