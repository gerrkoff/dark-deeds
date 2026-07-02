using System.IdentityModel.Tokens.Jwt;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

// TokenService.Serialize scopes the token audience: an explicit audience (used by the
// OAuth/MCP access-token path) overrides the default, while omitting it falls back to the
// configured Auth:Audience (used by the login/sign-in/renew path).
public class TokenServiceTests
{
    private readonly AuthSettings _authSettings = new()
    {
        Issuer = "dd-issuer",
        Audience = "dd-audience",
        Key = "unit-test-signing-key-that-is-long-enough-for-hmacsha256",
        Lifetime = 60,
    };

    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _tokenService = new TokenService(Options.Create(_authSettings), new ClaimsService());
    }

    [Fact]
    public void Serialize_WithMcpAudience_SetsAudClaimToOAuthAccessAudience()
    {
        // Act
        var token = _tokenService.Serialize(CreateBuildInfo(), audience: OAuthConstants.AccessTokenAudience);

        // Assert
        Assert.Equal(OAuthConstants.AccessTokenAudience, ReadAudience(token));
    }

    [Fact]
    public void Serialize_WithoutAudience_SetsAudClaimToConfiguredAudience()
    {
        // Act
        var token = _tokenService.Serialize(CreateBuildInfo());

        // Assert
        Assert.Equal(_authSettings.Audience, ReadAudience(token));
    }

    private static AuthTokenBuildInfo CreateBuildInfo()
    {
        return new AuthTokenBuildInfo
        {
            UserId = "user-42",
            Username = "test-user",
            DisplayName = "Test User",
        };
    }

    private static string ReadAudience(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Aud).Value;
    }
}
