namespace DD.App;

public static class Program
{
    public static void Main(string[] args)
    {
        Logging.SafeRunHost(() => CreateHostBuilder(args).Build().Run());
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseLogging(Startup.App)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}
