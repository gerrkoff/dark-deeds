using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

public class AuthCodeServiceTests
{
    private const string TokenIssuer = "dd-oauth";
    private const string AuthCodeAudience = "dd-oauth-authcode";
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
    public void IssueThenVerify_ValidCode_RoundTripsData()
    {
        // Arrange
        var data = CreateData();

        // Act
        var code = _service.Issue(data);
        var verified = _service.Verify(code, ClientId, RedirectUri);

        // Assert
        Assert.NotNull(verified);
        Assert.Equal(data.UserId, verified.UserId);
        Assert.Equal(data.ClientId, verified.ClientId);
        Assert.Equal(data.RedirectUri, verified.RedirectUri);
        Assert.Equal(data.CodeChallenge, verified.CodeChallenge);
    }

    [Fact]
    public void Verify_TamperedSignature_ReturnsNull()
    {
        // Arrange
        var code = _service.Issue(CreateData());
        var tampered = TamperSignature(code);

        // Act
        var verified = _service.Verify(tampered, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public void Verify_CodeSignedWithDifferentKey_ReturnsNull()
    {
        // Arrange
        var foreignCode = IssueWith(
            "different-signing-key-that-is-also-long-enough-abcdef",
            CreateData(),
            DateTime.UtcNow.AddMinutes(5));

        // Act
        var verified = _service.Verify(foreignCode, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public void Verify_ExpiredCode_ReturnsNull()
    {
        // Arrange
        var expiredCode = IssueWith(_authSettings.Key, CreateData(), DateTime.UtcNow.AddMinutes(-5));

        // Act
        var verified = _service.Verify(expiredCode, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public void Verify_ClientIdMismatch_ReturnsNull()
    {
        // Arrange
        var code = _service.Issue(CreateData());

        // Act
        var verified = _service.Verify(code, "another-client", RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    [Fact]
    public void Verify_RedirectUriMismatch_ReturnsNull()
    {
        // Arrange
        var code = _service.Issue(CreateData());

        // Act
        var verified = _service.Verify(code, ClientId, "http://127.0.0.1:5000/other");

        // Assert
        Assert.Null(verified);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    public void Verify_MalformedCode_ReturnsNull(string? code)
    {
        // Act
        var verified = _service.Verify(code!, ClientId, RedirectUri);

        // Assert
        Assert.Null(verified);
    }

    private static AuthCodeData CreateData()
    {
        return new AuthCodeData
        {
            UserId = "user-42",
            ClientId = ClientId,
            RedirectUri = RedirectUri,
            CodeChallenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM",
        };
    }

    private static string IssueWith(string key, AuthCodeData data, DateTime expires)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, data.UserId),
            new("client_id", data.ClientId),
            new("redirect_uri", data.RedirectUri),
            new("code_challenge", data.CodeChallenge),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = expires.AddMinutes(-10),
            IssuedAt = expires.AddMinutes(-10),
            Expires = expires,
            Issuer = TokenIssuer,
            Audience = AuthCodeAudience,
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
