using DarkDeeds.TaskServiceApp.ContractImpl.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TaskServiceApp.ContractImpl
{
    public static class DependencyInjectionExtensions
    {   
        public static void AddTaskServiceContractImpl(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}