using System.Security.Claims;
using DarkDeeds.Auth.Extensions;
using DarkDeeds.Auth.Models;

namespace DarkDeeds.Api.Controllers.Base
{
    public class BaseUserController : BaseController
    {
        protected CurrentUser GetUser()
        {
            return ((ClaimsIdentity) User.Identity).ToCurrentUser();
        }
    }
}