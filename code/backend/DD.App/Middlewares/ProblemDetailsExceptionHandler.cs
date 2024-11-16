using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DD.App.Middlewares;

public class ProblemDetailsExceptionHandler(bool isProduction)
{
    public async Task Handle(HttpContext context)
    {
        var problemDetailsService = context.RequestServices.GetService<IProblemDetailsService>();

        if (problemDetailsService is null)
        {
            return;
        }

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;
        var problemDetails = GetProblemDetails(exception);

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problemDetails,
        });
    }

    private ProblemDetails GetProblemDetails(Exception? exception)
    {
        if (isProduction)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = string.Empty,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            };
        }

        return exception switch
        {
            null => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = "Unknown exception",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Extensions = { ["StackTrace"] = exception.StackTrace },
            },
        };
    }
}
