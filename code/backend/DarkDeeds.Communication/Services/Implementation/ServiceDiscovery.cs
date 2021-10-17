using System;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using DarkDeeds.Communication.Services.Interface;

namespace DarkDeeds.Communication.Services.Implementation
{
    class ServiceDiscovery : IServiceDiscovery
    {
        private readonly IConsulClient _consul;

        public ServiceDiscovery(IConsulClient consul)
        {
            _consul = consul;
        }

        public async Task<Uri> GetService(string name)
        {
            var services = await _consul.Health.Service(name, string.Empty, true);
            var rand = new Random();
            var skipFew = rand.Next(services.Response.Length); // TODO: round robin
            var service = services.Response.Skip(skipFew).First().Service;

            return new Uri($"{service.Meta["scheme"]}://{service.Address}:{service.Port}");
        }
    }
}