using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace DarkDeeds.AppMetrics;

// TODO: remove

public static class Extensions
{
    public static void AddDarkDeedsAppMetrics(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MetricsSettings>(options => configuration.GetSection("metrics").Bind(options));
        services.AddHostedService<AppMetricsBackgroundService>();
    }

    public static void UseDarkDeedsAppMetrics(this IApplicationBuilder app)
    {
        app.UseHttpMetrics();
    }

    public static void UseDarkDeedsAppMetricsServer(this IApplicationBuilder app)
    {
        app.UseMetricServer();
    }

    public static void MapDarkDeedsAppMetrics(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapMetrics();
    }
}
