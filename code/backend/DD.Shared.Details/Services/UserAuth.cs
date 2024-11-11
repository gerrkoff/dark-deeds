using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace DD.Shared.Details.Services;

// [Route("api/web/[controller]")]
public interface IUserAuth
{
    bool IsAuthenticated();

    string UserId();

    AuthToken AuthToken();
}

internal sealed class UserAuth(
    IHttpContextAccessor httpContextAccessor,
    IClaimsService claimsService)
    : IUserAuth
{
    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    public string UserId()
    {
        return AuthToken().UserId;
    }

    public AuthToken AuthToken()
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        return claimsService.ToToken(httpContextAccessor.HttpContext.User.Claims);
    }
}
