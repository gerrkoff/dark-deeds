using System.Security.Claims;
using DD.Shared.Details.Abstractions.Models;

namespace DD.Shared.Details.Abstractions;

public interface IAuthTokenConverter
{
    ClaimsIdentity ToIdentity(AuthToken authToken);

    AuthToken FromPrincipal(ClaimsPrincipal identity);
}
