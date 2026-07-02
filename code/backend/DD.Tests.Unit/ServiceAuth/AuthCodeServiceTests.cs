using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Models;
using DD.ServiceAuth.Domain.OAuth.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

public class AuthCodeServiceTests
{
    private const string ClientId = "client-1";
    private const string RedirectUri = "http://127.0.0.1:5000/callback";

    private readonly AuthSettings _authSettings = new()
    {
        Issuer = "dd-issuer",
        Audience = "dd-audience",
        Key = "unit-test-signing-key-that-is-long-enough-for-hmacsha256",
        Lifetime = 60,
    };

    private readonly AuthCodeService _service;

    public AuthCodeServiceTests()
    {
        _service = new AuthCodeService(Options.Create(_authSettings));
    }

    [Fact]
    public async Task IssueThenVerify_ValidCode_RoundTripsData()
    {
        // Arrange
        var data = CreateData();

        // Act
        var code = await _service.IssueAsync(data);
        var verified = await _service.VerifyAsync(code, ClientId, RedirectUri);

        // Assert
        Assert.NotNull(verified);
        Assert.Equal(data.UserId, verified.UserId);
        Assert.Equal(data.ClientId, verified.ClientId);
        Assert.Equal(data.RedirectUri, verified.RedirectUri);
        Assert.Equal(data.CodeChallenge, verified.CodeChallenge);
    }

    [Fact]
    public async Task VerifyAsync_TamperedSignature_ReturnsNull()
    {
        // Arrange
        var code = await _service.IssueAsync(CreateData());
        var tampered = TamperSignature(code);

        // Act
        var verified = await _service.VerifyAsync(tampered, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task VerifyAsync_CodeSignedWithDifferentKey_ReturnsNull()
    {
        // Arrange
        var foreignCode = IssueWith(
            "different-signing-key-that-is-also-long-enough-abcdef",
            CreateData(),
            DateTime.UtcNow.AddMinutes(5));

        // Act
        var verified = await _service.VerifyAsync(foreignCode, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task VerifyAsync_ExpiredCode_ReturnsNull()
    {
        // Arrange
        var expiredCode = IssueWith(_authSettings.Key, CreateData(), DateTime.UtcNow.AddMinutes(-5));

        // Act
        var verified = await _service.VerifyAsync(expiredCode, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task VerifyAsync_ClientIdMismatch_ReturnsNull()
    {
        // Arrange
        var code = await _service.IssueAsync(CreateData());

        // Act
        var verified = await _service.VerifyAsync(code, "another-client", RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public async Task VerifyAsync_RedirectUriMismatch_ReturnsNull()
    {
        // Arrange
        var code = await _service.IssueAsync(CreateData());

        // Act
        var verified = await _service.VerifyAsync(code, ClientId, "http://127.0.0.1:5000/other");

        // Assert
        Assert.Null(verified);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    public async Task VerifyAsync_MalformedCode_ReturnsNull(string? code)
    {
        // Act
        var verified = await _service.VerifyAsync(code!, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    private static AuthCodeModel CreateData()
    {
        return new AuthCodeModel
        {
            UserId = "user-42",
            ClientId = ClientId,
            RedirectUri = RedirectUri,
            CodeChallenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM",
        };
    }

    private static string IssueWith(string key, AuthCodeModel model, DateTime expires)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, model.UserId),
            new("client_id", model.ClientId),
            new("redirect_uri", model.RedirectUri),
            new("code_challenge", model.CodeChallenge),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = expires.AddMinutes(-10),
            IssuedAt = expires.AddMinutes(-10),
            Expires = expires,
            Issuer = OAuthTokenKinds.Issuer,
            Audience = OAuthTokenKinds.AuthorizationCodeAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(tokenDescriptor));
    }

    private static string TamperSignature(string code)
    {
        var parts = code.Split('.');
        var signature = parts[2];

        // Tamper the first base64url char: all of its bits are significant, so the decoded
        // signature bytes always change. The last char carries padding bits, so flipping it
        // can leave the decoded bytes unchanged and the signature still valid.
        var firstChar = signature[0];
        parts[2] = (firstChar == 'A' ? 'B' : 'A') + signature[1..];
        return string.Join('.', parts);
    }
}
