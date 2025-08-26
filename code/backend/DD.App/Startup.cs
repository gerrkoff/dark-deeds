using DD.App.Middlewares;
using DD.Clients.Details;
using DD.ServiceAuth.Details;
using DD.ServiceTask.Details;
using DD.Shared.Data.Migrator;
using DD.Shared.Details;
using GerrKoff.Monitoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;

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
        services.AddTaskService();
        services.AddAuthService(Configuration);
        services.AddDdAuthentication(Configuration);
        services.AddClients(Configuration);

        // shared
        services.AddSharedDetails();
        services.AddSharedData(Configuration);
        services.AddSharedDataMigrator();

        // custom
        services.AddCompression();
        services.AddSwagger();

        services.AddHttpContextAccessor();
        services.AddHealthChecks();
        services.AddProblemDetails();
        services.AddControllers(options =>
        {
            var authRequired = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(authRequired));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRequestLogging();
        app.UseMetrics();
        app.UseExceptionHandler(x => x.Run(new ProblemDetailsExceptionHandler(env.IsProduction()).Handle));
        app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
        app.UseHsts();
        app.UseResponseCompression();

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
        app.UseDdAuthentication();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapClientsCustomRoutes(Configuration);
            endpoints.MapTaskServiceCustomRoutes();
        });
        app.UseDefaultFiles();
        app.UseStaticWithNotCachedIndex();
    }
}
