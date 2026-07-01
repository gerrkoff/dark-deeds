using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DD.ServiceAuth.Domain.Services;

public interface IAuthCodeService
{
    string Issue(AuthCodeData data);

    [SuppressMessage(
        "Design",
        "CA1054:URI-like parameters should not be strings",
        Justification = "OAuth redirect_uri must be compared by exact string match, not URI-normalized.")]
    AuthCodeData? Verify(string code, string clientId, string redirectUri);
}

internal sealed class AuthCodeService(IOptions<AuthSettings> authSettings) : IAuthCodeService
{
    private const string TokenIssuer = "dd-oauth";
    private const string TokenAudience = "dd-oauth-authcode";
    private const int LifetimeMinutes = 5;
    private const string ClientIdClaim = "client_id";
    private const string RedirectUriClaim = "redirect_uri";
    private const string CodeChallengeClaim = "code_challenge";

    private readonly AuthSettings _authSettings = authSettings.Value;

    public string Issue(AuthCodeData data)
    {
        var keyBytes = Encoding.ASCII.GetBytes(_authSettings.Key);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, data.UserId),
            new(ClientIdClaim, data.ClientId),
            new(RedirectUriClaim, data.RedirectUri),
            new(CodeChallengeClaim, data.CodeChallenge),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(LifetimeMinutes),
            Issuer = TokenIssuer,
            Audience = TokenAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public AuthCodeData? Verify(string code, string clientId, string redirectUri)
    {
        if (string.IsNullOrEmpty(code))
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

        try
        {
            var principal = tokenHandler.ValidateToken(code, validationParameters, out _);

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var tokenClientId = principal.FindFirst(ClientIdClaim)?.Value;
            var tokenRedirectUri = principal.FindFirst(RedirectUriClaim)?.Value;
            var codeChallenge = principal.FindFirst(CodeChallengeClaim)?.Value;

            if (userId is null || tokenClientId is null || tokenRedirectUri is null || codeChallenge is null)
            {
                return null;
            }

            if (!string.Equals(tokenClientId, clientId, StringComparison.Ordinal) ||
                !string.Equals(tokenRedirectUri, redirectUri, StringComparison.Ordinal))
            {
                return null;
            }

            return new AuthCodeData
            {
                UserId = userId,
                ClientId = tokenClientId,
                RedirectUri = tokenRedirectUri,
                CodeChallenge = codeChallenge,
            };
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
