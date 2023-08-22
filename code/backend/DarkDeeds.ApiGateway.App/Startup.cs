using System;
using DarkDeeds.AppMetrics;
using DarkDeeds.Common.Web;
using DarkDeeds.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.ServiceDiscovery.Providers;

namespace DarkDeeds.ApiGateway.App
{
    public class Startup
    {
        internal const string App = "api-gateway";
        11public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IpHashLoadBalancer IpHashLoadBalancer(IServiceProvider serviceProvider,
                DownstreamRoute _,
                IServiceDiscoveryProvider serviceDiscoveryProvider
            ) => new(serviceDiscoveryProvider.Get);

            services.AddOcelot()
                .AddCustomLoadBalancer(IpHashLoadBalancer)
                .AddConsul();
            services.AddDarkDeedsAppMetrics(Configuration);
            services.AddDarkDeedsAppRegistration(App, Configuration);
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLogging();
            app.UseHealthChecks("/healthcheck");
            app.UseWebSockets();
            app.UseDarkDeedsAppMetrics();
            app.UseDarkDeedsAppMetricsServer();
            app.UseOcelot().Wait();
        }
    }
}
