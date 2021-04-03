using DarkDeeds.AuthServiceApp.App.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.AuthServiceApp.App.Controllers.Base
{
    public class BaseController : ControllerBase
    {
        protected void Validate()
        {
            if (!ModelState.IsValid)
                throw new ModelValidationException(ModelState);
        }
    }
}