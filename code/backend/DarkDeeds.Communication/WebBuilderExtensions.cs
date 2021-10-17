using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.Communication
{
    public static class WebBuilderExtensions
    {
        public static void SplitHttpProtocolsIfDevelopment(this IWebHostBuilder webBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;
            if (isDevelopment)
            {   
                var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
                if (string.IsNullOrWhiteSpace(url))
                    throw new InvalidOperationException("Application Url must not be empty");

                var uri = new Uri(url);
                if (!string.Equals(uri.Scheme,"http"))
                    throw new InvalidOperationException("Application Url must be HTTP in dev mode");

                webBuilder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(uri.Port, o => o.Protocols = HttpProtocols.Http2);
                    options.ListenAnyIP(uri.Port * 10, o => o.Protocols = HttpProtocols.Http1);
                });
            }
        }

        public static void ForceHttp2IfNoTls(this IWebHostBuilder webBuilder)
        {
            var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("Application Url must not be empty");

            var uri = new Uri(url);
            if (string.Equals(uri.Scheme, "https"))
                return;

            webBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(uri.Port, o => o.Protocols = HttpProtocols.Http2);
            });
        }
    }
}