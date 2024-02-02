using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Implementation.CommandProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.TelegramClient.Domain
{
    public static class Setup
    {
        public static void AddTelegramClientDomain(this IServiceCollection services, IConfiguration configuration)
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
    }
}
