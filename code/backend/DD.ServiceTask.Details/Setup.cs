using DD.ServiceTask.Details.Data;
using DD.ServiceTask.Details.Data.EntityRepository;
using DD.ServiceTask.Details.Infrastructure;
using DD.ServiceTask.Details.Web.Hubs;
using DD.ServiceTask.Domain;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.ServiceTask.Details
{
    public static class Setup
    {
        public static void AddTaskService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTaskServiceWeb();
            services.AddTaskServiceInfrastructure();
            services.AddTaskServiceData(configuration);
            services.AddTaskServiceDomain();
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
            services.AddScoped<INotifierService, NotifierService>();
        }

        private static void AddTaskServiceData(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("tasksDb");

            if (connectionString == null)
                throw new InvalidOperationException("Connection string for tasksDb is not found");

            services.AddSingleton<IMongoDbContext>(_ => new MongoDbContext(connectionString));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IPlannedRecurrenceRepository, PlannedRecurrenceRepository>();
        }

        public static void MapTaskServiceCustomRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<TaskHub>("/ws/task/task");
        }


    }
}
