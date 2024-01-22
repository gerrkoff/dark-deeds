using DD.TaskService.Domain.Mapping;
using DD.TaskService.Domain.Services;
using DD.TaskService.Domain.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace DD.TaskService.Domain
{
    public static class Di
    {
        public static void AddTaskServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, Services.TaskService>();
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
