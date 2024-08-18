using System.Globalization;
using System.Security.Claims;
using DD.Shared.Details.Abstractions;

namespace DD.Shared.Details.Common;

internal sealed class AuthTokenConverter : IAuthTokenConverter
{
    public ClaimsIdentity ToIdentity(AuthToken authToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, authToken.Username),
            new(ClaimTypes.Sid, authToken.UserId),
            new(ClaimTypes.GivenName, authToken.DisplayName),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }

    public AuthToken FromPrincipal(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity?)principal.Identity;

        var expiration = identity?.FindFirst("exp")?.Value;

        var user = new AuthToken
        {
            Username = identity?.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value ?? string.Empty,
            UserId = identity?.FindFirst(ClaimTypes.Sid)?.Value ?? string.Empty,
            DisplayName = identity?.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
            Expires = !string.IsNullOrWhiteSpace(expiration)
                ? UnixTimeStampToDateTime(double.Parse(expiration, CultureInfo.InvariantCulture))
                : null,
        };

        return user;
    }

    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}
