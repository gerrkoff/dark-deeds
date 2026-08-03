using DD.ServiceTask.Domain.Services;
using DD.ServiceTask.Domain.Specifications;
using DD.Shared.TaskText;
using Microsoft.Extensions.DependencyInjection;

namespace DD.ServiceTask.Domain;

public static class Setup
{
    public static void AddTaskServiceDomain(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskParserService, TaskParserService>();
        services.AddScoped<ITaskTextParser, TaskTextParser>();
        services.AddScoped<ITaskTextDateProvider, TaskTextDateProvider>();
        services.AddScoped<IRecurrenceCreatorService, RecurrenceCreatorService>();
        services.AddScoped<IDateService, DateService>();
        services.AddScoped<IRecurrenceService, RecurrenceService>();
        services.AddScoped<ITaskSpecification, TaskSpecification>();
        services.AddScoped<IPlannedRecurrenceSpecification, PlannedRecurrenceSpecification>();
        services.AddScoped<ISpecificationFactory, SpecificationFactory>();
    }
}
