using DarkDeeds.WebClientBff.Services.Mapping;
using DarkDeeds.WebClientBff.Services.Services.Implementation;
using DarkDeeds.WebClientBff.Services.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBff.Services
{
    public static class DependencyInjectionExtensions
    {
        public static void AddWebClientBffServices(this IServiceCollection services)
        {
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddTransient<ITaskUpdatedListener, TaskUpdatedListener>();
            services.AddAutoMapper(typeof(ModelsMapping));
        }
    }
}