using DarkDeeds.ServiceAuth.ContractImpl.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceAuth.ContractImpl
{
    public static class DependencyInjectionExtensions
    {   
        public static void AddAuthServiceContractImpl(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}