using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Models;

namespace DD.ServiceAuth.Domain.Services;

internal sealed class ClaimsService : IClaimsService
{
    public IEnumerable<Claim> FromToken(AuthTokenBuildInfo authToken)
    {
        return [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, authToken.UserId),
            new(JwtRegisteredClaimNames.Name, authToken.Username),
            new(JwtRegisteredClaimNames.GivenName, authToken.DisplayName),
        ];
    }

    public AuthToken ToToken(IEnumerable<Claim> claims)
    {
        var claimList = claims.ToList();
        var tokenId = Guid.Parse(claimList.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value);
        var userId = claimList.Single(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        var username = claimList.Single(x => x.Type == JwtRegisteredClaimNames.Name).Value;
        var displayName = claimList.Single(x => x.Type == JwtRegisteredClaimNames.GivenName).Value;
        var expires = DateTimeOffset.FromUnixTimeSeconds(long.Parse(
            claimList.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value, CultureInfo.InvariantCulture));

        return new AuthToken
        {
            TokenId = tokenId,
            UserId = userId,
            Username = username,
            DisplayName = displayName,
            Expires = expires,
        };
    }
}
