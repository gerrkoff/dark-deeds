using DD.ServiceTask.Details.Data;
using DD.ServiceTask.Details.Subscriptions;
using DD.ServiceTask.Details.Web.Hubs;
using DD.ServiceTask.Domain;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.Shared.Details.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DD.ServiceTask.Details;

public static class Setup
{
    public static void AddTaskService(this IServiceCollection services)
    {
        services.AddTaskServiceWeb();
        services.AddTaskServiceInfrastructure();
        services.AddTaskServiceData();
        services.AddTaskServiceDomain();
    }

    public static void MapTaskServiceCustomRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<TaskHub>("/ws/task/task");
    }

    private static void AddTaskServiceWeb(this IServiceCollection services)
    {
        services.AddHostedService<HubHeartbeat<TaskHub>>();
        services.AddTransient<IUserIdProvider, HubUserIdProvider>();
        services.AddSignalR()
            .AddHubOptions<TaskHub>(options => options.EnableDetailedErrors = true);
    }

    private static void AddTaskServiceInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskServiceSubscriber, TaskServiceSubscriber>();
    }

    private static void AddTaskServiceData(this IServiceCollection services)
    {
        services.AddScoped<TaskRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<PlannedRecurrenceRepository>();
        services.AddScoped<IPlannedRecurrenceRepository, PlannedRecurrenceRepository>();
    }
}
