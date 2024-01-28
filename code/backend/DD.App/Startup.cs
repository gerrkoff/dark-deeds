using DD.ServiceAuth.Details;
using DD.ServiceTask.Details;
using DD.Shared.Auth;
using DD.Shared.Data;
using DD.Shared.Web;
using DD.TelegramClient.Details;
using DD.WebClientBff.Details;
using GerrKoff.Monitoring;
using GerrKoff.Monitoring.Misc;

namespace DD.App;

public class Startup(IConfiguration configuration)
{
    public const string App = "backend";

    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddDdAuthentication(Configuration);
        services.AddDdSharedWeb();
        services.AddDdSharedData(Configuration);

        services.AddAppApi();

        services.AddLoggingWeb();
        services.AddMetricsWeb(Configuration, Program.Meta);

        // features
        services.AddTaskService(Configuration);
        services.AddAuthService();
        services.AddTelegramClient(Configuration);
        services.AddWebClientBff();

        // shared
        services.AddSharedAuth();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRequestLogging();
        app.UseMetrics();
        app.UseUnhandledExceptionHandler(env);

        if (!env.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.Backend v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors(builder => builder
                .SetIsOriginAllowed(origin => origin.EndsWith(":3000"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapTelegramClientCustomRoutes(Configuration);
            endpoints.MapTaskServiceCustomRoutes();
        });
    }
}
