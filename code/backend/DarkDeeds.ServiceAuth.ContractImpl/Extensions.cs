using DarkDeeds.ServiceAuth.ContractImpl.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceAuth.ContractImpl
{
    public static class Extensions
    {   
        public static void AddAuthContractImpl(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}