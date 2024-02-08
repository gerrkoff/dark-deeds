using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DD.Shared.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DD.ServiceAuth.Domain.Services;

public interface ITokenService
{
    string Serialize(AuthToken authToken);
}

class TokenService(
    IOptions<AuthSettings> authSettings,
    IAuthTokenConverter authTokenConverter)
    : ITokenService
{
    private readonly AuthSettings _authSettings = authSettings.Value;

    public string Serialize(AuthToken authToken)
    {
        ClaimsIdentity identity = authTokenConverter.ToIdentity(authToken);

        var now = DateTime.UtcNow;
        var keyBytes = Encoding.ASCII.GetBytes(_authSettings.Key ?? string.Empty);
        var symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);
        var jwt = new JwtSecurityToken(
            _authSettings.Issuer,
            _authSettings.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(_authSettings.Lifetime)),
            signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }
}
