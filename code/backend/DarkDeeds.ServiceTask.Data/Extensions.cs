using DarkDeeds.ServiceTask.Data.EntityRepository;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.Data
{
    public static class Extensions
    {   
        public static void AddTaskDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("mongoDb");
            services.AddSingleton<IMongoDbContext>(_ => new MongoDbContext(connectionString));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IPlannedRecurrenceRepository, PlannedRecurrenceRepository>();
        }
    }
}