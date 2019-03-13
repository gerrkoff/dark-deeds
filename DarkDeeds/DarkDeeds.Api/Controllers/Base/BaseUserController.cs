using System.Security.Claims;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Models.Entity;

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