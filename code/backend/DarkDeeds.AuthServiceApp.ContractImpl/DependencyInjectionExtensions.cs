using DarkDeeds.AuthServiceApp.ContractImpl.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.AuthServiceApp.ContractImpl
{
    public static class DependencyInjectionExtensions
    {   
        public static void AddAuthServiceContractImpl(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}