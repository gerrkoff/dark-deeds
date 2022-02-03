using DarkDeeds.WebClientBff.UseCases.Mapping;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBff.UseCases
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