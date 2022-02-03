using DarkDeeds.WebClientBff.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers
{
    [ApiController]
    [Route("api/web/[controller]")]
    public abstract class BaseController : Controller
    {
        protected void Validate()
        {
            if (!ModelState.IsValid)
                throw new ModelValidationException(ModelState);
        }
    }
}