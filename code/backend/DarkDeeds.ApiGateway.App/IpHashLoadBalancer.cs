using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.Values;

namespace DarkDeeds.ApiGateway.App
{
    internal class IpHashLoadBalancer : ILoadBalancer
    {
        private readonly Func<Task<List<Service>>> _services;

        internal IpHashLoadBalancer(Func<Task<List<Service>>> services)
        {
            _services = services;
        }

        public async Task<Response<ServiceHostAndPort>> Lease(HttpContext httpContext)
        {
            var services = await _services();

            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            var hash = Math.Abs(ip.GetHashCode());
            var index = hash % services.Count;

            var next = services[index];
            return new OkResponse<ServiceHostAndPort>(next.HostAndPort);
        }

        public void Release(ServiceHostAndPort hostAndPort)
        {
        }
    }
}
