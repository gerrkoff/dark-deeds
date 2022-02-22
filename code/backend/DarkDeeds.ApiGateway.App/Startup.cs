using DarkDeeds.AppMetrics;
using DarkDeeds.Common.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace DarkDeeds.ApiGateway.App
{
    public class Startup
    {
        public const string App = "api-gateway";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot()
                .AddConsul();
            services.AddDarkDeedsAppMetrics(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLogging();
            app.UseWebSockets();
            app.UseDarkDeedsAppMetrics();
            app.UseDarkDeedsAppMetricsServer();
            app.UseOcelot().Wait();
        }
    }
}
