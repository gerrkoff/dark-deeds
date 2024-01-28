using GerrKoff.Monitoring;

namespace DD.App;

public static class Program
{
    public static void Main(string[] args)
    {
        Logging.RunSafe(() => CreateHostBuilder(args).Build().Run(), Meta.Version);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseLoggingWeb(Meta)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

    public static readonly AppMeta Meta = AppMeta.FromEnvironment(typeof(Program), "dark-deeds");
}
