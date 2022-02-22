using DarkDeeds.Common.Web;
using DarkDeeds.Communication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.ServiceAuth.App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Logging.SafeRunHost(() => CreateHostBuilder(args).Build().Run());
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseLogging(Startup.App)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ForceHttp2IfNoTls();
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
