using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.Common.Web
{
    public static class Extensions
    {
        public static void AddDarkDeedsTestControllers(this IServiceCollection services)
        {
            services.AddScoped<TestAttribute>();
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
}