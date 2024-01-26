using DD.Shared.Auth;
using Microsoft.AspNetCore.Http;

namespace DD.Shared.Web;

// [Route("api/web/[controller]")]
public interface IUserAuth
{
    bool IsAuthenticated();
    string UserId();
    AuthToken AuthToken();
}

class UserAuth(
    IHttpContextAccessor httpContextAccessor,
    IAuthTokenConverter authTokenConverter)
    : IUserAuth
{
    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public string UserId()
    {
        return AuthToken().UserId;
    }

    public AuthToken AuthToken()
    {
        return authTokenConverter.FromPrincipal(httpContextAccessor.HttpContext.User);
    }
}
