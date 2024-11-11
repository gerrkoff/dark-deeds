using System.Security.Claims;
using DD.Shared.Details.Abstractions.Models;

namespace DD.Shared.Details.Abstractions;

public interface IClaimsService
{
    IEnumerable<Claim> FromToken(AuthTokenBuildInfo authToken);

    AuthToken ToToken(IEnumerable<Claim> claims);
}
