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
            var servicesResponse = await _consul.Health.Service(name, string.Empty, true);
            var services = servicesResponse.Response.Where(x => !x.Service.Tags.Any()).ToList();
            var rand = new Random();
            var skipFew = rand.Next(services.Count); // TODO: round robin
            var service = services.Skip(skipFew).First().Service;

            return new Uri($"{service.Meta["scheme"]}://{service.Address}:{service.Port}");
        }
    }
}