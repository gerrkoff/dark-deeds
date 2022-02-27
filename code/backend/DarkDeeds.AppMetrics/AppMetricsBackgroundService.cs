using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus.DotNetRuntime;

namespace DarkDeeds.AppMetrics
{
    class AppMetricsBackgroundService : IHostedService
    {
        private readonly ILogger<AppMetricsBackgroundService> _logger;
        private readonly MetricsSettings _options;

        private IDisposable _metrics;

        public AppMetricsBackgroundService(
            ILogger<AppMetricsBackgroundService> logger,
            IOptions<MetricsSettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_options.Enabled)
                return Task.CompletedTask;

            _logger.LogDebug("Configuring prometheus-net.DotNetRuntime");

            var builder = DotNetRuntimeStatsBuilder.Default();

            if (_options.Verbose)
            {
                builder = DotNetRuntimeStatsBuilder.Customize()
                    .WithContentionStats(CaptureLevel.Informational)
                    .WithGcStats(CaptureLevel.Verbose)
                    .WithThreadPoolStats(CaptureLevel.Informational)
                    .WithExceptionStats(CaptureLevel.Errors)
                    .WithJitStats();
            }

            builder.WithErrorHandler(ex =>
                _logger.LogError(ex, "Unexpected exception occurred in prometheus-net.DotNetRuntime"));

            if (_options.Debug)
            {
                _logger.LogInformation("Using debugging metrics");
                builder.WithDebuggingMetrics(true);
            }

            _logger.LogDebug("Starting prometheus-net.DotNetRuntime");

            _metrics = builder.StartCollecting();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _metrics?.Dispose();
            return Task.CompletedTask;
        }
    }
}
