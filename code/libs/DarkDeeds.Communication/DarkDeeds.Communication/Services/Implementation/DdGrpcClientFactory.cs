using System;
using System.Net.Http;
using System.Threading.Tasks;
using DarkDeeds.Communication.Interceptors;
using DarkDeeds.Communication.Services.Interface;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;

namespace DarkDeeds.Communication.Services.Implementation
{
    class DdGrpcClientFactory<T> : IDdGrpcClientFactory<T>
    {
        private readonly string _name;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly AuthInterceptor _authInterceptor;

        public DdGrpcClientFactory(string name, IServiceDiscovery serviceDiscovery, AuthInterceptor authInterceptor)
        {
            _name = name;
            _serviceDiscovery = serviceDiscovery;
            _authInterceptor = authInterceptor;
        }

        public async Task<T> Create()
        {
            var uri = await _serviceDiscovery.GetService(_name);

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(httpClientHandler);

            var channel = GrpcChannel.ForAddress(uri, new GrpcChannelOptions
            {
                HttpClient = httpClient
            });

            var callInvoker = channel.Intercept(_authInterceptor);

            return (T) Activator.CreateInstance(typeof(T), callInvoker);
        }
    }
}