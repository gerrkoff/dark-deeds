using DarkDeeds.ServiceTask.Services.Implementation;
using DarkDeeds.ServiceTask.Services.Interface;
using DarkDeeds.ServiceTask.Services.Mapping;
using DarkDeeds.ServiceTask.Services.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.Services
{
    public static class Extensions
    {
        public static void AddTaskServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskParserService, TaskParserService>();
            services.AddScoped<IRecurrenceCreatorService, RecurrenceCreatorService>();
            services.AddScoped<IDateService, DateService>();
            services.AddScoped<IRecurrenceService, RecurrenceService>();
            services.AddScoped<ITaskSpecification, TaskSpecification>();
            services.AddScoped<IPlannedRecurrenceSpecification, PlannedRecurrenceSpecification>();
            services.AddScoped<ISpecificationFactory, SpecificationFactory>();

            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}
