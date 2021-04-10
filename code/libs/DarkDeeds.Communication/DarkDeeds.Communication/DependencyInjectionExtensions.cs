using System.Net.Http;
using DarkDeeds.Communication.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.Communication
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpClientBuilder ConfigureGrpcClient(this IHttpClientBuilder builder)
        {
            return builder
                .AddInterceptor<AuthInterceptor>()
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
        }
        
        public static void AddGrpcClientServices(this IServiceCollection services)
        {
            services.AddTransient<AuthInterceptor>();
        }
    }
}