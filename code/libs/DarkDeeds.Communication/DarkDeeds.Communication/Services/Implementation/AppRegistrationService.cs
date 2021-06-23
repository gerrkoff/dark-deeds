using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using DarkDeeds.Communication.Services.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Communication.Services.Implementation
{
    class AppRegistrationService : IHostedService
    {
        private readonly IConsulClient _consul;
        private readonly ILogger<AppRegistrationService> _logger;
        private readonly string _appName;
        private readonly IAddressService _addressService;

        public AppRegistrationService(IConsulClient consul, ILogger<AppRegistrationService> logger, string appName, IAddressService addressService)
        {
            _consul = consul;
            _logger = logger;
            _appName = appName;
            _addressService = addressService;
        }

        private string _id;
        private string GetId() => _id ??= Guid.NewGuid().ToString();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var id = GetId();
            if (!_addressService.TryGetAddress(out Uri uri))
                return;
            
            var registration = new AgentServiceRegistration()
            {
                ID = id,
                Name = _appName,
                Address = uri.Host,
                Port = uri.Port,
                Meta = new Dictionary<string, string> {{"scheme", uri.Scheme}},
                Check = new AgentServiceCheck
                {
                    TCP = $"{uri.Host}:{uri.Port}",
                    Interval = new TimeSpan(0, 0, 10),
                    Timeout = new TimeSpan(0, 0, 3),
                    DeregisterCriticalServiceAfter = new TimeSpan(0, 0, 20),
                }
            };
            
            _logger.LogInformation(
                $"Register service [{registration.Name}] at [{registration.Address}:{registration.Port}] as [{registration.ID}]");

            try
            {
                await _consul.Agent.ServiceRegister(registration, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Failed to register app, error: {e.Message}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var id = GetId();
            _logger.LogInformation($"Deregister service [{id}]");
            try
            {
                await _consul.Agent.ServiceDeregister(id, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Failed to deregister app, error: {e.Message}");
            }
        }
    }
}