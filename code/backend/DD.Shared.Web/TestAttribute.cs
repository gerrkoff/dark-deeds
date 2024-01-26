using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace DD.Shared.Web;

class TestAttribute(IConfiguration configuration) : ActionFilterAttribute
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
