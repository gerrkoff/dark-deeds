using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DD.ServiceAuth.Domain.Services;

public interface ITokenService
{
    string Serialize(AuthTokenBuildInfo authToken);
}

internal sealed class TokenService(
    IOptions<AuthSettings> authSettings,
    IClaimsService claimsService)
    : ITokenService
{
    private readonly AuthSettings _authSettings = authSettings.Value;

    public string Serialize(AuthTokenBuildInfo authToken)
    {
        var keyBytes = Encoding.ASCII.GetBytes(_authSettings.Key);
        var claims = claimsService.FromToken(authToken);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_authSettings.Lifetime),
            Issuer = _authSettings.Issuer,
            Audience = _authSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
