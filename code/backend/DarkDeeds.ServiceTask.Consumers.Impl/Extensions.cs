using DarkDeeds.Communication;
using DarkDeeds.ServiceTask.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.Consumers.Impl
{
    public static class Extensions
    {
        public static void AddTaskServiceApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service", configuration);
            services.AddDarkDeedsGrpcClientFactory<RecurrenceService.RecurrenceServiceClient>("task-service", configuration);
            services.AddDarkDeedsGrpcClientFactory<ParserService.ParserServiceClient>("task-service", configuration);
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();

            services.AddAutoMapper(typeof(ModelsMapping));
        }
    }
}
