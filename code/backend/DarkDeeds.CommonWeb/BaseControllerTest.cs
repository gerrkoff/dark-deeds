using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.CommonWeb
{
    [AllowAnonymous]
    [Route("api/test")]
    [ServiceFilter(typeof(TestAttribute))]
    public abstract class BaseControllerTest : ControllerBase
    {
    }
}