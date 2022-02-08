using DarkDeeds.ServiceTask.ContractImpl.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.ContractImpl
{
    public static class Extensions
    {   
        public static void AddTaskServiceContractImpl(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}