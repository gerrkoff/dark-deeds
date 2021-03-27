using DarkDeeds.Authentication;
using DarkDeeds.Authentication.Models;

namespace DarkDeeds.Api.Controllers.Base
{
    public class BaseUserController : BaseController
    {
        protected CurrentUser GetUser() => User.ToCurrentUser();
    }
}