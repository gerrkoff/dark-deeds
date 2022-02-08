using DarkDeeds.Communication;
using DarkDeeds.ServiceAuth.Contract;
using DarkDeeds.ServiceTask.Contract;
using DarkDeeds.WebClientBff.Communication.Apps;
using DarkDeeds.WebClientBff.Communication.Mapping;
using DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBff.Communication
{
    public static class Extensions
    {
        public static void AddWebClientBffCommunications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsGrpcClientFactory<AuthService.AuthServiceClient>("auth-service", configuration);
            services.AddScoped<IAuthServiceApp, AuthServiceApp>();
            
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service", configuration);
            services.AddDarkDeedsGrpcClientFactory<RecurrenceService.RecurrenceServiceClient>("task-service", configuration);
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();

            services.AddAutoMapper(typeof(AuthServiceModelsMapping));
        }
    }
}