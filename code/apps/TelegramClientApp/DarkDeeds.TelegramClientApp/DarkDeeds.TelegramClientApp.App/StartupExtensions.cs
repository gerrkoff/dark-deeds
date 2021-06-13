using DarkDeeds.Common.Misc;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
        }

        public static void AddTelegramClientCommunications(this IServiceCollection services)
        {
            services.AddDarkDeedsGrpcClientFactory<TaskService.TaskServiceClient>("task-service");
            services.AddScoped<ITaskServiceApp, Communication.TaskServiceApp>();
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }

        public static void AddTelegramClientDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsTelegramClientContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsTelegramClientContext>();
        }

        public static void AddTelegramClientApi(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                var authRequired = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(authRequired));
            });

            var buildInfo = new BuildInfo(typeof(Startup), null);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DarkDeeds.TelegramClient",
                    Version = buildInfo.AppVersion
                });
            });
        }
    }
}