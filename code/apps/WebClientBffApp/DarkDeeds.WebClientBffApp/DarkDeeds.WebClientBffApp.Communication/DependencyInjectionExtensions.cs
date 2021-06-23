using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.Communication;
using DarkDeeds.TaskServiceApp.Contract;
using DarkDeeds.WebClientBffApp.Communication.Apps;
using DarkDeeds.WebClientBffApp.Communication.Mapping;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public static class DependencyInjectionExtensions
    {
        public static void AddWebClientBffCommunications(this IServiceCollection services)
        {
            services.AddDarkDeedsGrpcClientFactory<AuthService.AuthServiceClient>("auth-service");
            services.AddScoped<IAuthServiceApp, Apps.AuthServiceApp>();
            
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service");
            services.AddDarkDeedsGrpcClientFactory<RecurrenceService.RecurrenceServiceClient>("task-service");
            services.AddScoped<ITaskServiceApp, Apps.TaskServiceApp>();
            
            services.AddDarkDeedsHttpClientFactory();
            services.AddScoped<ITelegramClientApp, TelegramClientApp>();

            services.AddAutoMapper(typeof(AuthServiceModelsMapping));
        }
    }
}