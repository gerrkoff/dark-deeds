using System.Runtime.CompilerServices;
using DarkDeeds.TelegramClient.Services.Implementation;
using DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Interface.CommandProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("DarkDeeds.TelegramClient.Tests")]

namespace DarkDeeds.TelegramClient.Services
{
    public static class Extensions
    {
        public static void AddTelegramClientServices(this IServiceCollection services, IConfiguration configuration)
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