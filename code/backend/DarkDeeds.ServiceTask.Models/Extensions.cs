using DarkDeeds.ServiceTask.Models.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.Models
{
    public static class Extensions
    {
        public static void AddTaskAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}