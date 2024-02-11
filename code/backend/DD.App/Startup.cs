using DD.App.Dto;
using DD.ServiceAuth.Details;
using DD.ServiceTask.Details;
using DD.Shared.Auth;
using DD.Shared.Data;
using DD.Shared.Web;
using DD.TelegramClient.Details;
using DD.WebClientBff.Details;
using GerrKoff.Monitoring;
using GerrKoff.Monitoring.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

namespace DD.App;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        // monitoring
        services.AddLoggingWeb();
        services.AddMetricsWeb(Configuration, Program.Meta);

        // features
        services.AddTaskService(Configuration);
        services.AddAuthService();
        services.AddTelegramClient(Configuration);
        services.AddWebClientBff();

        // shared
        services.AddSharedAuth();
        services.AddDdSharedWeb();
        services.AddDdSharedData(Configuration);

        services.AddDdAuthentication(Configuration);

        services.AddHttpContextAccessor();
        services.AddHealthChecks();
        services.AddControllers(options =>
        {
            var authRequired = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(authRequired));
        });

        var buildInfo = new BuildInfoDto(typeof(Startup));
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DarkDeeds.Backend",
                Version = buildInfo.AppVersion
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRequestLogging();
        app.UseMetrics();
        app.UseUnhandledExceptionHandler(env);
        app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
        app.UseHsts();

        if (!env.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.Backend v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors(builder => builder
                .SetIsOriginAllowed(origin => origin.EndsWith(":3000", StringComparison.Ordinal))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        }

        app.UseHealthChecks("/healthcheck");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapTelegramClientCustomRoutes(Configuration);
            endpoints.MapTaskServiceCustomRoutes();
        });
        app.UseDefaultFiles();
        app.UseStaticFiles();
    }
}
