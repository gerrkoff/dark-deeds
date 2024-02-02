using System.Security.Claims;

namespace DD.Shared.Auth;

public interface IAuthTokenConverter
{
    ClaimsIdentity ToIdentity(AuthToken authToken);
    AuthToken FromPrincipal(ClaimsPrincipal identity);
}

class AuthTokenConverter : IAuthTokenConverter
{
    public ClaimsIdentity ToIdentity(AuthToken authToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, authToken.Username),
            new(ClaimTypes.Sid, authToken.UserId),
            new(ClaimTypes.GivenName, authToken.DisplayName)
        };

        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }

    public AuthToken FromPrincipal(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity) principal.Identity;

        var user = new AuthToken
        {
            Username = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value,
            UserId = identity.FindFirst(ClaimTypes.Sid).Value,
            DisplayName = identity.FindFirst(ClaimTypes.GivenName).Value
        };

        string expiration = identity.FindFirst("exp")?.Value;
        if (!string.IsNullOrWhiteSpace(expiration))
            user.Expires = UnixTimeStampToDateTime(double.Parse(expiration));

        return user;
    }

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}
