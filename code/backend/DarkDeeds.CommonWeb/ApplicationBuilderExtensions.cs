using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.CommonWeb
{
    public static class ApplicationBuilderExtensions
    {
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