using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

// End-to-end composition of the OAuth primitives: an authorization code plus a PKCE
// verifier yields an access JWT (validatable with the Auth signing key) and a refresh
// token, and that refresh token in turn mints another usable access JWT. The access-token
// step mirrors AuthService.CreateAccessTokenAsync, which delegates to
// ITokenService.SerializeWithLifetime.
public class TokenFlowTests
{
    private const string CodeVerifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
    private const string CodeChallenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM";
    private const string UserId = "user-42";
    private const string ClientId = "client-1";
    private const string RedirectUri = "http://127.0.0.1:5000/callback";
    private const int AccessTokenLifetimeMinutes = 60;

    private readonly AuthSettings _authSettings = new()
    {
        Issuer = "dd-issuer",
        Audience = "dd-audience",
        Key = "unit-test-signing-key-that-is-long-enough-for-hmacsha256",
        Lifetime = 2880,
    };

    private readonly OAuthSettings _oauthSettings = new()
    {
        AccessTokenLifetimeMinutes = AccessTokenLifetimeMinutes,
        RefreshTokenLifetimeDays = 30,
        ScopesSupported = ["mcp"],
    };

    private readonly AuthCodeService _authCodeService;
    private readonly PkceService _pkceService = new();
    private readonly RefreshTokenService _refreshTokenService;
    private readonly TokenService _tokenService;

    public TokenFlowTests()
    {
        var authOptions = Options.Create(_authSettings);
        _authCodeService = new AuthCodeService(authOptions);
        _refreshTokenService = new RefreshTokenService(authOptions, Options.Create(_oauthSettings));
        _tokenService = new TokenService(authOptions, new ClaimsService());
    }

    [Fact]
    public async Task AuthorizationCode_ComposesAccessAndRefreshTokens_AndRefreshMintsAnotherAccessToken()
    {
        // Arrange - the authorization server issued a code embedding the PKCE challenge.
        var code = _authCodeService.Issue(new AuthCodeData
        {
            UserId = UserId,
            ClientId = ClientId,
            RedirectUri = RedirectUri,
            CodeChallenge = CodeChallenge,
        });

        // Act & Assert - authorization_code grant.
        var codeData = _authCodeService.Verify(code, ClientId, RedirectUri);
        Assert.NotNull(codeData);
        Assert.True(_pkceService.Verify(CodeVerifier, codeData.CodeChallenge));

        var accessToken = CreateAccessToken(codeData.UserId);
        var refreshToken = await _refreshTokenService.IssueAsync(new RefreshTokenData(codeData.UserId, ClientId));

        Assert.Equal(UserId, ReadValidatedSubject(accessToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));

        // Act & Assert - refresh_token grant reuses the refresh token to mint a new access JWT.
        var refreshData = await _refreshTokenService.VerifyAsync(refreshToken);
        Assert.NotNull(refreshData);

        var refreshedAccessToken = CreateAccessToken(refreshData.UserId);
        Assert.Equal(UserId, ReadValidatedSubject(refreshedAccessToken));
        Assert.NotEqual(accessToken, refreshedAccessToken);
    }

    private string CreateAccessToken(string userId)
    {
        var buildInfo = new AuthTokenBuildInfo
        {
            UserId = userId,
            Username = "test-user",
            DisplayName = "Test User",
        };

        return _tokenService.SerializeWithLifetime(buildInfo, AccessTokenLifetimeMinutes);
    }

    private string ReadValidatedSubject(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _authSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _authSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        var principal = handler.ValidateToken(accessToken, validationParameters, out _);
        return principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
    }
}
