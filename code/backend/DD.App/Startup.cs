using DD.App.Dto;
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
using Microsoft.OpenApi.Models;

namespace DD.App;

public class Startup(IConfiguration configuration)
{
    private readonly bool isAutoForwarded = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED"), "true", StringComparison.OrdinalIgnoreCase);

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

        var buildInfo = new BuildInfoDto(typeof(Startup));
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DarkDeeds.Backend",
                Version = buildInfo.AppVersion,
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    []
                },
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRequestLogging();
        app.UseMetrics();
        app.UseExceptionHandler(x => x.Run(new ProblemDetailsExceptionHandler(env.IsProduction()).Handle));

        if (!isAutoForwarded)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
        }

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
        app.UseDdAuthentication();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapClientsCustomRoutes(Configuration);
            endpoints.MapTaskServiceCustomRoutes();
        });
        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var maxAge = ctx.File.Name.Equals("index.html", StringComparison.Ordinal) ? 300 : 31536000;
                ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={maxAge}");
            },
        });
    }
}
