using DarkDeeds.WebClientBffApp.Services.Mapping;
using DarkDeeds.WebClientBffApp.Services.Services.Implementation;
using DarkDeeds.WebClientBffApp.Services.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.Services
{
    public static class DependencyInjectionExtensions
    {
        public static void AddWebClientBffServices(this IServiceCollection services)
        {
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddAutoMapper(typeof(ModelsMapping));
        }
    }
}