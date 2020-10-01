using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;

namespace DarkDeeds.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                //NLog: catch setup errors
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    var env = hostingContext.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                        .AddJsonFile($"settings/appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .UseNLog();
    }
}