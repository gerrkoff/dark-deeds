using System;
using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.Communication;
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
        private const string CommunicationConfigKey = "Communication";
        public static void AddWebClientBffCommunications(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(CommunicationConfigKey).Get<CommunicationSettings>();
            
            services.Configure<CommunicationSettings>(
                options => configuration.GetSection(CommunicationConfigKey).Bind(options));
            
            services.AddGrpcClientServices();

            services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
                    o.Address = new Uri(settings.AuthService))
                .ConfigureGrpcClient();

            services.AddScoped<IAuthServiceApp, Apps.AuthServiceApp>();
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            services.AddScoped<ITelegramClientApp, TelegramClientApp>();

            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }
    }
}