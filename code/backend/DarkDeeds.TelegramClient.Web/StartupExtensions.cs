using DarkDeeds.Communication;
using DarkDeeds.ServiceTask.Contract;
using DarkDeeds.TelegramClient.Communication;
using DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClient.Services.Implementation;
using DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Interface.CommandProcessor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TelegramClient.Web
{
    public static class StartupExtensions
    {
        public static void AddTelegramClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTelegramClientServices(configuration);
            services.AddTelegramClientCommunications(configuration);
        }
        
        public static void MapTelegramClientCustomRoutes(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            endpoints.MapControllerRoute("bot", $"api/tlgm/bot/{configuration["Bot"]}",
                new { controller = "Bot", action = "Process" });
        }

        private static void AddTelegramClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<IDateService, DateService>();
            services.AddScoped<IBotCommandParserService, BotCommandParserService>();
            services.AddScoped<IShowTodoCommandProcessor, ShowTodoCommandProcessor>();
            services.AddScoped<ICreateTaskCommandProcessor, CreateTaskCommandProcessor>();
            services.AddScoped<IStartCommandProcessor, StartCommandProcessor>();
            services.AddScoped<IBotProcessMessageService, BotProcessMessageService>();
            services.AddScoped<ITestService, TestService>();

            if (bool.TryParse(configuration["EnableTelegramIntegration"], out bool v) && v)
            {
                services.AddScoped<IBotSendMessageService>(_ => new BotSendMessageService(configuration["Bot"]));
            }
            else
            {
                services.AddScoped<IBotSendMessageService>(_ => new BotSendMessageDebugService(configuration["Bot"]));
            }
        }

        private static void AddTelegramClientCommunications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service", configuration);
            services.AddDarkDeedsGrpcClientFactory<ParserService.ParserServiceClient>("task-service", configuration);
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}