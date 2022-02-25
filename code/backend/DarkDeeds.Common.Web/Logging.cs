using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace DarkDeeds.Common.Web
{
    // TODO: loki retention policy
    // https://grafana.com/docs/loki/latest/configuration/
    // https://grafana.com/docs/loki/latest/operations/storage/retention/
    // TODO: fix issue with Parent/Span Ids
    // https://github.com/dotnet/runtime/issues/41072
    public static class Logging
    {
        private const string Empty = "---";
        public static void SafeRunHost(Action run)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web host");
                run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, string app) =>
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.WithSpan()
                    .Enrich.FromLogContext()
                    .WriteTo.Console();

                var lokiConnectionString = context.Configuration.GetConnectionString(EnvConstants.ConnectionStringLoki);
                var serviceDiscoveryHost = Environment.GetEnvironmentVariable(EnvConstants.ServiceDiscoveryHost) ?? Empty;
                var serviceDiscoveryPort = Environment.GetEnvironmentVariable(EnvConstants.ServiceDiscoveryPort) ?? Empty;
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Empty;


                if (!string.IsNullOrWhiteSpace(lokiConnectionString))
                    configuration.WriteTo.GrafanaLoki(
                        lokiConnectionString,
                        new List<LokiLabel>
                        {
                            new() { Key = "app", Value = app },
                            new() { Key = "host", Value = $"{serviceDiscoveryHost}:{serviceDiscoveryPort}" },
                            new() { Key = "env", Value = environment },
                        },
                        createLevelLabel: true);
            });

        public static void UseRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
        }
    }
}
