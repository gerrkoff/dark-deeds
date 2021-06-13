using System;
using Consul;
using DarkDeeds.Communication.Interceptors;
using DarkDeeds.Communication.Services.Implementation;
using DarkDeeds.Communication.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DarkDeeds.Communication
{
    public static class DiExtensions
    {
        public static void AddDarkDeedsAppRegistration(this IServiceCollection services, string appName)
        {
            services.AddDarkDeedsServiceDiscovery();

            services.AddTransient<IAddressService, AddressService>();
            services.AddHostedService(x =>
                ActivatorUtilities.CreateInstance<AppRegistrationService>(x, appName));
        }
        
        public static void AddDarkDeedsGrpcClientFactory<T>(this IServiceCollection services, string name)
        {
            services.AddDarkDeedsServiceDiscovery();
            
            services.AddHttpContextAccessor();
            services.TryAddTransient<AuthInterceptor>();

            services.AddTransient<IDdGrpcClientFactory<T>>(x =>
                ActivatorUtilities.CreateInstance<DdGrpcClientFactory<T>>(x, name)
            );
        }

        public static void AddDarkDeedsHttpClientFactory(this IServiceCollection services)
        {
            services.AddDarkDeedsServiceDiscovery();

            services.AddTransient<IDdHttpClientFactory, DdHttpClientFactory>();
        }

        private static void AddDarkDeedsServiceDiscovery(this IServiceCollection services)
        {
            services.TryAddSingleton<IConsulClient>(_ => new ConsulClient(config =>
            {
                var serviceDiscoveryConsul = Environment.GetEnvironmentVariable(EnvVariables.ConsulUri);
                if (string.IsNullOrWhiteSpace(serviceDiscoveryConsul))
                    serviceDiscoveryConsul = "http://localhost:8500";
                config.Address = new Uri(serviceDiscoveryConsul);
            }));

            services.TryAddSingleton<IServiceDiscovery, ServiceDiscovery>();
        }
    }
}