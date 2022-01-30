using DarkDeeds.Communication;
using DarkDeeds.TaskServiceApp.Contract;
using DarkDeeds.TelegramClientApp.Communication;
using DarkDeeds.TelegramClientApp.Data.Context;
using DarkDeeds.TelegramClientApp.Entities;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Services.Implementation;
using DarkDeeds.TelegramClientApp.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Services.Interface.CommandProcessor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TelegramClientApp.Web
{
    public static class StartupExtensions
    {
        public static void AddTelegramClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTelegramClientIdentity();
            services.AddTelegramClientServices(configuration);
            services.AddTelegramClientCommunications(configuration);
            services.AddTelegramClientDatabase(configuration);
        }
        
        public static void MapTelegramClientCustomRoutes(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            endpoints.MapControllerRoute("bot", $"api/tlgm/bot/{configuration["Bot"]}",
                new { controller = "Bot", action = "Process" });
        }

        private static void AddTelegramClientIdentity(this IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<UserEntity>();
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DarkDeedsTelegramClientContext>();
            
            services.AddScoped<UserManager<UserEntity>>();
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
            services.AddScoped<ITaskServiceApp, Communication.TaskServiceApp>();
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }

        private static void AddTelegramClientDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsTelegramClientContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsTelegramClientContext>();
        }
    }
}