using DarkDeeds.Communication;
using DarkDeeds.ServiceTask.Contract;
using DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TelegramClient.Communication
{
    public static class Extensions
    {
        public static void AddTelegramClientCommunications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service", configuration);
            services.AddDarkDeedsGrpcClientFactory<ParserService.ParserServiceClient>("task-service", configuration);
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}