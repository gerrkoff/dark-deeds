using DD.App.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

namespace DD.App;

static class StartupExtensions
{
    public static void AddAppApi(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            var authRequired = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(authRequired));
        });

        var buildInfo = new BuildInfoDto(typeof(Startup), null);
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DarkDeeds.Backend",
                Version = buildInfo.AppVersion
            });
        });
    }

    public static void UseDarkDeedsExceptionHandler(this IApplicationBuilder app, bool isProduction)
    {
        app.UseExceptionHandler(x =>
        {
            x.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                if (isProduction)
                {
                    await context.Response.WriteAsJsonAsync(new { Message = "Something went wrong" });
                    return;
                }

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature == null)
                {
                    await context.Response.WriteAsJsonAsync(new { Message = "Failed to get exception" });
                    return;
                }

                var exception = exceptionHandlerPathFeature.Error;
                await context.Response.WriteAsJsonAsync(new Dictionary<string, string>
                {
                    { "Type", exception.GetType().ToString() },
                    { "Message", exception.Message },
                    { "StackTrace", exception.StackTrace }
                });
            });
        });
    }
}
