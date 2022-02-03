using DarkDeeds.Communication;
using DarkDeeds.ServiceAuth.Contract;
using DarkDeeds.ServiceTask.Contract;
using DarkDeeds.WebClientBffApp.Communication.Apps;
using DarkDeeds.WebClientBffApp.Communication.Mapping;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public static class DependencyInjectionExtensions
    {
        public static void AddWebClientBffCommunications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsGrpcClientFactory<AuthService.AuthServiceClient>("auth-service", configuration);
            services.AddScoped<IAuthServiceApp, AuthServiceApp>();
            
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service", configuration);
            services.AddDarkDeedsGrpcClientFactory<RecurrenceService.RecurrenceServiceClient>("task-service", configuration);
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            
            services.AddDarkDeedsHttpClientFactory(configuration);
            services.AddScoped<ITelegramClientApp, TelegramClientApp>();

            services.AddAutoMapper(typeof(AuthServiceModelsMapping));
        }
    }
}