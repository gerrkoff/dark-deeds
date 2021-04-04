using AutoMapper;
using DarkDeeds.WebClientBffApp.App.Hubs;
using DarkDeeds.WebClientBffApp.Communication;
using DarkDeeds.WebClientBffApp.Data.Context;
using DarkDeeds.WebClientBffApp.Data.Repository;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Data;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using DarkDeeds.WebClientBffApp.Services.Implementation;
using DarkDeeds.WebClientBffApp.Services.Interface;
using DarkDeeds.WebClientBffApp.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.App
{
    public static class StartupExtensions
    {
        public static void AddWebClientBffServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepositoryNonDeletable<>), typeof(RepositoryNonDeletable<>));
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<ITaskHubService, TaskHubService>();
            
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            services.AddScoped<IAuthServiceApp, AuthServiceApp>();
            services.AddScoped<ITelegramClientApp, TelegramClientApp>();
            services.AddHttpContextAccessor();
            services.Configure<CommunicationSettings>(
                options => configuration.GetSection("Communication").Bind(options));
        }
        
        public static void AddWebClientBffAutoMapper(this IServiceCollection services)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ModelsMappingProfile>();
            });
        }

        public static void AddWebClientBffDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsWebClientBffContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsWebClientBffContext>();
        }
    }
}