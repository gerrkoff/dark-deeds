using System;
using Consul;
using DarkDeeds.Communication.Amqp.Common;
using DarkDeeds.Communication.Amqp.Publish;
using DarkDeeds.Communication.Amqp.Subscribe;
using DarkDeeds.Communication.Interceptors;
using DarkDeeds.Communication.Services.Implementation;
using DarkDeeds.Communication.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DarkDeeds.Communication
{
    public static class DiExtensions
    {
        private static void AddDarkDeedsAmpqServices(this IServiceCollection services)
        {
            services.TryAddTransient<IChannelProvider, ChannelProvider>();
            services.TryAddTransient<IAmqpItemSerializer, AmqpItemSerializer>();
            services.TryAddTransient(typeof(IPublisher<>), typeof(Publisher<>));
            services.TryAddTransient(typeof(ISubscriber<>), typeof(Subscriber<>));
        }
        
        public static void AddDarkDeedsAmpqPublisher<TService, TImplementation, TPayload>(this IServiceCollection services)
            where TImplementation : AbstractMessagePublisher<TPayload>, TService
        {
            services.AddDarkDeedsAmpqServices();
            services.AddSingleton(typeof(TService), typeof(TImplementation));
        }
        
        public static void AddDarkDeedsAmpqSubscriber<TService, TPayload>(this IServiceCollection services)
            where TService : AbstractMessageSubscriber<TPayload>
        {
            services.AddDarkDeedsAmpqServices();
            services.AddHostedService<TService>();
        }
        
        public static void AddDarkDeedsAppRegistration(this IServiceCollection services, string appName, IConfiguration configuration)
        {
            services.AddDarkDeedsServiceDiscovery(configuration);

            services.AddTransient<IAddressService, AddressService>();
            services.AddHostedService(x =>
                ActivatorUtilities.CreateInstance<AppRegistrationService>(x, appName));
        }
        
        public static void AddDarkDeedsGrpcClientFactory<T>(this IServiceCollection services, string name, IConfiguration configuration)
        {
            services.AddDarkDeedsServiceDiscovery(configuration);
            
            services.AddHttpContextAccessor();
            services.TryAddTransient<AuthInterceptor>();

            services.AddTransient<IDdGrpcClientFactory<T>>(x =>
                ActivatorUtilities.CreateInstance<DdGrpcClientFactory<T>>(x, name)
            );
        }

        public static void AddDarkDeedsHttpClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsServiceDiscovery(configuration);

            services.AddTransient<IDdHttpClientFactory, DdHttpClientFactory>();
        }

        private static void AddDarkDeedsServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceDiscoveryConsul = configuration?.GetConnectionString(Constants.ConnectionStringConsul) ??
                                         "http://localhost:8500";

            services.TryAddSingleton<IConsulClient>(_ =>
                new ConsulClient(config => config.Address = new Uri(serviceDiscoveryConsul)));

            services.TryAddSingleton<IServiceDiscovery, ServiceDiscovery>();
        }
    }
}