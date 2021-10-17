using DarkDeeds.WebClientBffApp.App.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers.Base
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