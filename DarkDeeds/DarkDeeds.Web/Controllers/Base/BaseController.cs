using DarkDeeds.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers.Base
{
    public class BaseController : Controller
    {
        protected void Validate()
        {
            if (!ModelState.IsValid)
                throw new ModelValidationException(ModelState);
        }
    }
}