using System.Security.Claims;

namespace DD.Shared.Details.Abstractions;

public interface IAuthTokenConverter
{
    ClaimsIdentity ToIdentity(AuthToken authToken);

    AuthToken FromPrincipal(ClaimsPrincipal identity);
}
