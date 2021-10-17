using DarkDeeds.Communication;
using Microsoft.AspNetCore.Hosting;
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
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ForceHttp2IfNoTls();
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}