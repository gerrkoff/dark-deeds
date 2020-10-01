using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Api.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionHandlerFilter> _logger;

        public ExceptionHandlerFilter(ILogger<ExceptionHandlerFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, string.Empty);

            context.Result = new ObjectResult(new
            {
                Message = "An error has occured."
            }) {StatusCode = 500};
            
#if DEBUG   
            context.Result = new ObjectResult(context.Exception) {StatusCode = 500};
#endif
        }
    }
}