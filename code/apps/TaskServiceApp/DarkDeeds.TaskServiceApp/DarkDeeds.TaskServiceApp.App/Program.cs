using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.TaskServiceApp.App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (isDevelopment)
                    {
                        // Setup a separate HTTP/2 endpoint without TLS.
                        webBuilder.ConfigureKestrel(options =>
                        {
                            options.ListenLocalhost(50011, o =>
                                o.Protocols = HttpProtocols.Http1);
                            options.ListenLocalhost(50012, o =>
                                o.Protocols = HttpProtocols.Http2);
                        });
                    }

                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}