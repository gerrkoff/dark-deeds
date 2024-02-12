using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace DD.Shared.Web;

internal sealed class TestAttribute(
    [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "We don't need to access the configuration from the attribute. We just need to inject it.")]
    IConfiguration configuration)
    : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (AreTestHandlerEnabled())
        {
            base.OnActionExecuting(context);
        }
        else
        {
            context.Result = new NotFoundResult();
        }
    }

    private bool AreTestHandlerEnabled()
    {
        return bool.TryParse(configuration["EnableTestHandlers"], out var v) && v;
    }
}
