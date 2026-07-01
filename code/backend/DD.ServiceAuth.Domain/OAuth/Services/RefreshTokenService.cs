using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DD.ServiceAuth.Domain.OAuth.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DD.ServiceAuth.Domain.OAuth.Services;

public interface IRefreshTokenService
{
    Task<string> IssueAsync(RefreshTokenModel model);

    Task<RefreshTokenModel?> VerifyAsync(string refreshToken);
}

internal sealed class RefreshTokenService(
    IOptions<AuthSettings> authSettings,
    IOptions<OAuthSettings> oauthSettings)
    : IRefreshTokenService
{
    private const string TokenIssuer = "dd-oauth";
    private const string TokenAudience = "dd-oauth-refresh";
    private const string ClientIdClaim = "client_id";

    private readonly AuthSettings _authSettings = authSettings.Value;
    private readonly OAuthSettings _oauthSettings = oauthSettings.Value;

    public Task<string> IssueAsync(RefreshTokenModel model)
    {
        var keyBytes = Encoding.ASCII.GetBytes(_authSettings.Key);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, model.UserId),
            new(ClientIdClaim, model.ClientId),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_oauthSettings.RefreshTokenLifetimeDays),
            Issuer = TokenIssuer,
            Audience = TokenAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    public async Task<RefreshTokenModel?> VerifyAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        var keyBytes = Encoding.ASCII.GetBytes(_authSettings.Key);
        var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TokenIssuer,
            ValidateAudience = true,
            ValidAudience = TokenAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        var result = await tokenHandler.ValidateTokenAsync(refreshToken, validationParameters);
        if (!result.IsValid)
        {
            return null;
        }

        var userId = result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var clientId = result.ClaimsIdentity.FindFirst(ClientIdClaim)?.Value;

        if (userId is null || clientId is null)
        {
            return null;
        }

        return new RefreshTokenModel(userId, clientId);
    }
}
