using DarkDeeds.WebClientBffApp.UseCases.Mapping;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.UseCases
{
    public static class DependencyInjectionExtensions
    {
        public static void AddWebClientBffUseCases(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ModelsMapping));
            services.AddAutoMapper(typeof(ModelsMapping));
        }
    }
}