using DarkDeeds.AppMetrics;
using DD.ServiceAuth.Details;
using DD.ServiceTask.Details;
using DD.Shared.Auth;
using DD.Shared.Data;
using DD.Shared.Web;
using DD.TelegramClient.Details;
using DD.WebClientBff.Details;

namespace DD.App;

public class Startup(IConfiguration configuration)
{
    public const string App = "backend";

    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddDarkDeedsAuthentication(Configuration);
        services.AddDarkDeedsAppMetrics(Configuration);
        services.AddDarkDeedsTestControllers();
        services.AddBackendApi();
        services.AddBackendDatabase(Configuration);

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
        app.UseRouting();
        app.UseDarkDeedsAppMetrics();
        app.UseDarkDeedsExceptionHandler(env.IsProduction());

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDarkDeedsAppMetrics();
            endpoints.MapTelegramClientCustomRoutes(Configuration);
            endpoints.MapTaskServiceCustomRoutes();
        });
    }
}
