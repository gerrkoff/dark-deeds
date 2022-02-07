using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using DarkDeeds.Communication.Common;
using DarkDeeds.Communication.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Communication.Services.Implementation
{
    class AppRegistrationService : AbstractRegisterBackgroundService<HttpRequestException>
    {
        private readonly IConsulClient _consul;
        private readonly ILogger<AppRegistrationService> _logger;
        private readonly IAddressService _addressService;
        private readonly string _appName;
        private readonly bool _withMetricsPort;

        public AppRegistrationService(IConsulClient consul,
            ILogger<AppRegistrationService> logger,
            IAddressService addressService,
            string appName,
            bool withMetricsPort) :
            base(logger)
        {
            _consul = consul;
            _logger = logger;
            _addressService = addressService;
            _appName = appName;
            _withMetricsPort = withMetricsPort;
        }

        private string _id;
        private string GetId() => _id ??= Guid.NewGuid().ToString();

        protected override Func<CancellationToken, Task> CreateRegisterJob()
        {
            if (!_addressService.TryGetAddress(out Uri uri))
                return null;
            
            var id = $"{uri.Host}:{uri.Port}";
            
            var registration = new AgentServiceRegistration
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
                    Timeout = new TimeSpan(0, 0, 5),
                    DeregisterCriticalServiceAfter = new TimeSpan(24, 0, 0),
                }
            };

            AgentServiceRegistration registrationMetrics = null;
            if (_withMetricsPort)
                registrationMetrics = new AgentServiceRegistration
                {
                    ID = $"{id}-metrics",
                    Name = _appName,
                    Address = uri.Host,
                    Port = uri.Port * 10,
                    Meta = new Dictionary<string, string> { { "scheme", uri.Scheme } },
                    Tags = new[] { "metrics" },
                    Check = new AgentServiceCheck
                    {
                        TCP = $"{uri.Host}:{uri.Port}",
                        Interval = new TimeSpan(0, 0, 10),
                        Timeout = new TimeSpan(0, 0, 3),
                        DeregisterCriticalServiceAfter = new TimeSpan(0, 0, 20),
                    }
                };

            return async cancellationToken =>
            {
                await _consul.Agent.ServiceRegister(registration, cancellationToken);
                _logger.LogInformation(
                    $"Register service [{registration.Name}] at [{registration.Address}:{registration.Port}] as [{registration.ID}]");

                if (registrationMetrics == null)
                    return;
                
                await _consul.Agent.ServiceRegister(registrationMetrics, cancellationToken);
                _logger.LogInformation(
                    $"Register service metrics [{registration.Name}] at [{registration.Address}:{registration.Port}] as [{registration.ID}]");
            };
        }
    }
}