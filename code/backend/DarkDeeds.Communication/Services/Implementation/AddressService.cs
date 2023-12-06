using System;
using System.Net;
using System.Net.Sockets;
using DarkDeeds.Common;
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
            var serviceDiscoveryPort = Environment.GetEnvironmentVariable(EnvConstants.ServiceDiscoveryPort);
            var serviceDiscoveryHost = Environment.GetEnvironmentVariable(EnvConstants.ServiceDiscoveryHost);
            if (!string.IsNullOrWhiteSpace(serviceDiscoveryPort) && !string.IsNullOrWhiteSpace(serviceDiscoveryHost))
            {
                uri = new Uri($"http://{serviceDiscoveryHost}:{serviceDiscoveryPort}");
                return true;
            }

            var developmentUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (!string.IsNullOrWhiteSpace(developmentUrl))
            {
                uri = new Uri(developmentUrl);
                if (string.Equals(uri.Host, "0.0.0.0"))
                    uri = new Uri($"{uri.Scheme}://{GetLocalIpAddress()}:{uri.Port}");

                _logger.LogInformation($"Fallback to development url {uri}");
                return true;
            }

            _logger.LogWarning("Could not find address for service discovery");
            uri = null;
            return false;
        }

        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    var ipStr = ip.ToString();

                    if (ipStr.StartsWith("192.168."))
                        return ipStr;
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        // TODO: remove
        // private string GetLocalIPv4()
        // {
        //     string output = "";
        //     var q = NetworkInterface.GetAllNetworkInterfaces();
        //     foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()
        //         .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
        //         .Where(x => x.OperationalStatus == OperationalStatus.Up))
        //     {
        //         foreach (UnicastIPAddressInformation ip in item.GetIPProperties()
        //             .UnicastAddresses
        //             .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
        //         {
        //             output = ip.Address.ToString();
        //         }
        //     }
        //
        //     return output;
        // }
    }
}
