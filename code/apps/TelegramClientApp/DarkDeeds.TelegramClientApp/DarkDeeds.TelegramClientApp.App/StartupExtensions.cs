using DarkDeeds.Services.Interface;
using DarkDeeds.TelegramClientApp.Communication;
using DarkDeeds.TelegramClientApp.Data.Context;
using DarkDeeds.TelegramClientApp.Entities;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Infrastructure.Services;
using DarkDeeds.TelegramClientApp.Services.Implementation;
using DarkDeeds.TelegramClientApp.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Services.Interface.CommandProcessor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TelegramClientApp.App
{
    public static class StartupExtensions
    {
        public static void AddTelegramClientIdentity(this IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<UserEntity>();
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DarkDeedsTelegramClientContext>();
            
            services.AddScoped<UserManager<UserEntity>>();
        }

        public static void AddTelegramClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<IDateService, DateService>();
            services.AddScoped<IBotCommandParserService, BotCommandParserService>();
            services.AddScoped<IShowTodoCommandProcessor, ShowTodoCommandProcessor>();
            services.AddScoped<ICreateTaskCommandProcessor, CreateTaskCommandProcessor>();
            services.AddScoped<IStartCommandProcessor, StartCommandProcessor>();
            services.AddScoped<IBotProcessMessageService, BotProcessMessageService>();
            services.AddScoped<IBotSendMessageService>(_ => new BotSendMessageService(configuration["Bot"]));
#if DEBUG
            services.AddScoped<IBotSendMessageService>(_ => new BotSendMessageDebugService(configuration["Bot"]));
#endif
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            services.AddScoped<ITaskHubService, TaskHubService>();
            services.Configure<CommunicationSettings>(
                options => configuration.GetSection("Communication").Bind(options));
        }

        public static void AddTelegramClientDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsTelegramClientContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsTelegramClientContext>();
        }
    }
}