using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Services;
using DD.ServiceAuth.Domain.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

// The SPA-served consent flow rests on two OAuthFlowService contracts: GET /authorize hands off to
// the styled SPA at the configured base URL (preserving the query), and completion mints an
// authorization code only for the "allow" action while "deny" yields access_denied without issuing
// anything. These tests lock down both, using the real OAuth primitives (as TokenFlowTests does).
public class OAuthFlowServiceTests
{
    private const string UserId = "user-42";
    private const string ClientId = "client-1";
    private const string RedirectUri = "http://127.0.0.1:5000/callback";
    private const string CodeChallenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM";
    private const string State = "state-xyz";
    private const string ConsentRedirectBaseUrl = "http://localhost:3000";

    private readonly OAuthFlowService _service;

    public OAuthFlowServiceTests()
    {
        var authSettings = Options.Create(new AuthSettings
        {
            Issuer = "dd-issuer",
            Audience = "dd-audience",
            Key = "unit-test-signing-key-that-is-long-enough-for-hmacsha256",
            Lifetime = 2880,
        });

        var oauthSettings = Options.Create(new OAuthSettings
        {
            AccessTokenLifetimeMinutes = 60,
            RefreshTokenLifetimeDays = 30,
            ScopesSupported = ["mcp"],
            IssuerBaseUrl = "http://localhost:5000",
            ConsentRedirectBaseUrl = ConsentRedirectBaseUrl,
        });

        _service = new OAuthFlowService(
            new Mock<IAuthService>().Object,
            new AuthCodeService(authSettings),
            new PkceService(),
            new RefreshTokenService(authSettings, oauthSettings),
            new OAuthUrlService(),
            oauthSettings);
    }

    [Fact]
    public void BuildConsentRedirect_PrefixesConfiguredBaseUrl_AndPreservesQuery()
    {
        // Arrange
        const string queryString = "?response_type=code&client_id=x&state=y";

        // Act
        var redirect = _service.BuildConsentRedirect(queryString);

        // Assert
        Assert.Equal($"{ConsentRedirectBaseUrl}/{queryString}", redirect);
    }

    [Fact]
    public async Task BuildAuthorizeRedirectAsync_Allow_IssuesCodeAndBuildsSuccessRedirect()
    {
        // Act
        var redirect = await _service.BuildAuthorizeRedirectAsync(
            OAuthConstants.ActionAllow, UserId, ClientId, RedirectUri, CodeChallenge, State);

        // Assert
        Assert.StartsWith($"{RedirectUri}?", redirect, StringComparison.Ordinal);
        Assert.Contains("code=", redirect, StringComparison.Ordinal);
        Assert.Contains($"state={State}", redirect, StringComparison.Ordinal);
        Assert.DoesNotContain("error=", redirect, StringComparison.Ordinal);
    }

    [Fact]
    public async Task BuildAuthorizeRedirectAsync_Deny_ReturnsAccessDeniedWithoutIssuingCode()
    {
        // Act
        var redirect = await _service.BuildAuthorizeRedirectAsync(
            "deny", UserId, ClientId, RedirectUri, CodeChallenge, State);

        // Assert
        Assert.StartsWith($"{RedirectUri}?", redirect, StringComparison.Ordinal);
        Assert.Contains($"error={OAuthConstants.AccessDeniedError}", redirect, StringComparison.Ordinal);
        Assert.Contains($"state={State}", redirect, StringComparison.Ordinal);
        Assert.DoesNotContain("code=", redirect, StringComparison.Ordinal);
    }
}
