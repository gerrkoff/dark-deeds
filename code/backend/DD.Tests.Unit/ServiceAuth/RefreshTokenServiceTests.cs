using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

public class RefreshTokenServiceTests
{
    private const string TokenIssuer = "dd-oauth";
    private const string RefreshAudience = "dd-oauth-refresh";
    private const string UserId = "user-42";
    private const string ClientId = "client-1";

    private readonly AuthSettings _authSettings = new()
    {
        Issuer = "dd-issuer",
        Audience = "dd-audience",
        Key = "unit-test-signing-key-that-is-long-enough-for-hmacsha256",
        Lifetime = 60,
    };

    [Fact]
    public async Task IssueThenVerify_ValidToken_RoundTripsData()
    {
        // Arrange
        var service = CreateService();
        var data = new RefreshTokenData(UserId, ClientId);

        // Act
        var token = await service.IssueAsync(data);
        var verified = await service.VerifyAsync(token);

        // Assert
        Assert.NotNull(verified);
        Assert.Equal(UserId, verified.UserId);
        Assert.Equal(ClientId, verified.ClientId);
    }

    [Fact]
    public async Task VerifyAsync_TamperedSignature_ReturnsNull()
    {
        // Arrange
        var service = CreateService();
        var token = await service.IssueAsync(new RefreshTokenData(UserId, ClientId));
        var tampered = TamperSignature(token);

        // Act
        var verified = await service.VerifyAsync(tampered);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task VerifyAsync_TokenSignedWithDifferentKey_ReturnsNull()
    {
        // Arrange
        var service = CreateService();
        var foreignToken = IssueWith(
            "different-signing-key-that-is-also-long-enough-abcdef",
            DateTime.UtcNow.AddDays(30));

        // Act
        var verified = await service.VerifyAsync(foreignToken);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task VerifyAsync_ExpiredToken_ReturnsNull()
    {
        // Arrange - a correctly signed token whose validity window is entirely in the past.
        var service = CreateService();
        var expiredToken = IssueWith(_authSettings.Key, DateTime.UtcNow.AddMinutes(-5));

        // Act
        var verified = await service.VerifyAsync(expiredToken);

        // Assert
        Assert.Null(verified);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    public async Task VerifyAsync_MalformedToken_ReturnsNull(string? token)
    {
        // Arrange
        var service = CreateService();

        // Act
        var verified = await service.VerifyAsync(token!);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task RefreshGrant_IssuesNewVerifiableToken()
    {
        // Arrange
        var service = CreateService();
        var original = await service.IssueAsync(new RefreshTokenData(UserId, ClientId));

        // Act - simulate the refresh grant: verify the presented token, then mint a new one.
        var verifiedOriginal = await service.VerifyAsync(original);
        Assert.NotNull(verifiedOriginal);
        var renewed = await service.IssueAsync(new RefreshTokenData(verifiedOriginal.UserId, verifiedOriginal.ClientId));
        var verifiedRenewed = await service.VerifyAsync(renewed);

        // Assert
        Assert.NotNull(verifiedRenewed);
        Assert.Equal(UserId, verifiedRenewed.UserId);
        Assert.Equal(ClientId, verifiedRenewed.ClientId);
    }

    private RefreshTokenService CreateService()
    {
        var oauthSettings = new OAuthSettings
        {
            AccessTokenLifetimeMinutes = 60,
            RefreshTokenLifetimeDays = 30,
            ScopesSupported = ["mcp"],
        };

        return new RefreshTokenService(Options.Create(_authSettings), Options.Create(oauthSettings));
    }

    private static string IssueWith(string key, DateTime expires)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, UserId),
            new("client_id", ClientId),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = expires.AddMinutes(-10),
            IssuedAt = expires.AddMinutes(-10),
            Expires = expires,
            Issuer = TokenIssuer,
            Audience = RefreshAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(tokenDescriptor));
    }

    private static string TamperSignature(string token)
    {
        var parts = token.Split('.');
        var signature = parts[2];
        var lastChar = signature[^1];
        parts[2] = signature[..^1] + (lastChar == 'A' ? 'B' : 'A');
        return string.Join('.', parts);
    }
}
