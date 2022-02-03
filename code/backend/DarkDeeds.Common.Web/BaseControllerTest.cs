using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Common.Web
{
    [AllowAnonymous]
    [Route("api/test")]
    [ServiceFilter(typeof(TestAttribute))]
    public abstract class BaseControllerTest : ControllerBase
    {
    }
}