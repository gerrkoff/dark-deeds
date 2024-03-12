using DD.TelegramClient.Domain.Services;
using DD.TelegramClient.Domain.Services.CommandProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.TelegramClient.Domain;

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
        services.AddHttpClient();

        if (bool.TryParse(configuration["EnableTelegramIntegration"], out var v) && v)
            services.AddScoped<IBotSendMessageService, BotSendMessageService>();
        else
            services.AddScoped<IBotSendMessageService, BotSendMessageDebugService>();
    }
}
