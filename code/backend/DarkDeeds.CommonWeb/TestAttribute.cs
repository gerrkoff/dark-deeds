using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.CommonWeb
{
    public class TestAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;

        public TestAttribute(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
            return bool.TryParse(_configuration["EnableTestHandlers"], out bool v) && v;
        }
    }
}
