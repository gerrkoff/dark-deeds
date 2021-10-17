using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DarkDeeds.Communication.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Communication.Services.Implementation
{
    class AddressService : IAddressService
    {
        private readonly ILogger<AddressService> _logger;

        public AddressService(ILogger<AddressService> logger)
        {
            _logger = logger;
        }

        public bool TryGetAddress(out Uri uri)
        {
            var serviceDiscoveryPort = Environment.GetEnvironmentVariable(Constants.ServiceDiscoveryPort);
            var serviceDiscoveryHost = Environment.GetEnvironmentVariable(Constants.ServiceDiscoveryHost);
            if (!string.IsNullOrWhiteSpace(serviceDiscoveryPort) && !string.IsNullOrWhiteSpace(serviceDiscoveryHost))
            {
                uri = new Uri($"http://{serviceDiscoveryHost}:{serviceDiscoveryPort}");
                return true;
            }
            
            var developmentUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (!string.IsNullOrWhiteSpace(developmentUrl))
            {
                _logger.LogInformation("Fallback to development url");
                uri = new Uri(developmentUrl);
                if (string.Equals(uri.Host, "0.0.0.0"))
                    uri = new Uri($"{uri.Scheme}://{GetLocalIPv4()}:{uri.Port}");

                return true;
            }
            
            _logger.LogWarning("Could not find address for service discovery");
            uri = null;
            return false;
        }

        private string GetLocalIPv4()
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Where(x => x.OperationalStatus == OperationalStatus.Up))
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties()
                    .UnicastAddresses
                    .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    output = ip.Address.ToString();
                }
            }

            return output;
        }
    }
}