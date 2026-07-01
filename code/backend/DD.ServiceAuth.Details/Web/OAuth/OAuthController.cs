using System.Diagnostics.CodeAnalysis;
using DD.ServiceAuth.Domain;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SignInResult = DD.ServiceAuth.Domain.Enums.SignInResult;

namespace DD.ServiceAuth.Details.Web.OAuth;

[AllowAnonymous]
[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect_uri must be preserved and compared as an exact string, not URI-normalized.")]
public sealed class OAuthController(
    IAuthService authService,
    IAuthCodeService authCodeService,
    IPkceService pkceService,
    IRefreshTokenService refreshTokenService,
    IOptions<OAuthSettings> oauthSettings)
    : ControllerBase
{
    private const string GrantAuthorizationCode = "authorization_code";
    private const string GrantRefreshToken = "refresh_token";
    private const string ResponseTypeCode = "code";
    private const string CodeChallengeMethodS256 = "S256";
    private const string ActionAllow = "allow";

    private readonly OAuthSettings _oauthSettings = oauthSettings.Value;

    [HttpGet("/.well-known/oauth-authorization-server")]
    public IActionResult Metadata()
    {
        var baseUrl = BaseUrl();
        var metadata = new AuthServerMetadata(
            baseUrl,
            $"{baseUrl}/authorize",
            $"{baseUrl}/token",
            $"{baseUrl}/register",
            [ResponseTypeCode],
            [GrantAuthorizationCode, GrantRefreshToken],
            [CodeChallengeMethodS256],
            ["none"],
            _oauthSettings.ScopesSupported);

        return Ok(metadata);
    }

    [HttpGet("/authorize")]
    public IActionResult Authorize(
        [FromQuery(Name = "response_type")] string? responseType,
        [FromQuery(Name = "client_id")] string? clientId,
        [FromQuery(Name = "redirect_uri")] string? redirectUri,
        [FromQuery(Name = "code_challenge")] string? codeChallenge,
        [FromQuery(Name = "code_challenge_method")] string? codeChallengeMethod,
        [FromQuery(Name = "state")] string? state,
        [FromQuery(Name = "scope")] string? scope)
    {
        if (!IsLoopbackRedirectUri(redirectUri))
        {
            return BadRequest(new OAuthError("invalid_request", "redirect_uri must be a loopback address."));
        }

        if (!string.Equals(responseType, ResponseTypeCode, StringComparison.Ordinal))
        {
            return BadRequest(new OAuthError("unsupported_response_type", "response_type must be 'code'."));
        }

        if (string.IsNullOrEmpty(codeChallenge) ||
            !string.Equals(codeChallengeMethod, CodeChallengeMethodS256, StringComparison.Ordinal))
        {
            return BadRequest(new OAuthError(
                "invalid_request", "A PKCE code_challenge with code_challenge_method=S256 is required."));
        }

        if (string.IsNullOrEmpty(state))
        {
            return BadRequest(new OAuthError("invalid_request", "state is required."));
        }

        var html = ConsentPage.Render(
            clientId ?? string.Empty,
            redirectUri!,
            codeChallenge,
            state,
            scope ?? string.Empty);

        return Content(html, "text/html; charset=utf-8");
    }

    [HttpPost("/authorize")]
    public async Task<IActionResult> AuthorizeConsent(
        [FromForm(Name = "action")] string? action,
        [FromForm(Name = "username")] string? username,
        [FromForm(Name = "password")] string? password,
        [FromForm(Name = "client_id")] string? clientId,
        [FromForm(Name = "redirect_uri")] string? redirectUri,
        [FromForm(Name = "code_challenge")] string? codeChallenge,
        [FromForm(Name = "state")] string? state)
    {
        if (!IsLoopbackRedirectUri(redirectUri))
        {
            return BadRequest(new OAuthError("invalid_request", "redirect_uri must be a loopback address."));
        }

        if (string.IsNullOrEmpty(codeChallenge) || string.IsNullOrEmpty(state))
        {
            return BadRequest(new OAuthError("invalid_request", "code_challenge and state are required."));
        }

        if (!string.Equals(action, ActionAllow, StringComparison.Ordinal))
        {
            return RedirectWithError(redirectUri!, state);
        }

        var effectiveUsername = username ?? string.Empty;
        var signInResult = await authService.SignInAsync(new SignInInfoDto
        {
            Username = effectiveUsername,
            Password = password ?? string.Empty,
        });

        if (signInResult.Result != SignInResult.Success)
        {
            return RedirectWithError(redirectUri!, state);
        }

        var userId = await authService.GetUserIdAsync(effectiveUsername);
        var code = authCodeService.Issue(new AuthCodeData
        {
            UserId = userId,
            ClientId = clientId ?? string.Empty,
            RedirectUri = redirectUri!,
            CodeChallenge = codeChallenge,
        });

        var separator = redirectUri!.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        var location =
            $"{redirectUri}{separator}code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}";

        return Redirect(location);
    }

    [HttpPost("/token")]
    public async Task<IActionResult> Token(
        [FromForm(Name = "grant_type")] string? grantType,
        [FromForm(Name = "code")] string? code,
        [FromForm(Name = "redirect_uri")] string? redirectUri,
        [FromForm(Name = "client_id")] string? clientId,
        [FromForm(Name = "code_verifier")] string? codeVerifier,
        [FromForm(Name = "refresh_token")] string? refreshToken)
    {
        Response.Headers.CacheControl = "no-store";

        return grantType switch
        {
            GrantAuthorizationCode =>
                await ExchangeAuthorizationCodeAsync(code, redirectUri, clientId, codeVerifier),
            GrantRefreshToken =>
                await ExchangeRefreshTokenAsync(refreshToken),
            _ => BadRequest(new OAuthError(
                "unsupported_grant_type", "grant_type must be 'authorization_code' or 'refresh_token'.")),
        };
    }

    [HttpPost("/register")]
    public IActionResult Register([FromBody] ClientRegistrationRequest? request)
    {
        var redirectUris = request?.RedirectUris;
        if (redirectUris is null || redirectUris.Count == 0 || redirectUris.Any(uri => !IsLoopbackRedirectUri(uri)))
        {
            return BadRequest(new OAuthError("invalid_redirect_uri", "All redirect_uris must be loopback addresses."));
        }

        var clientId = Guid.NewGuid().ToString("N");
        var response = new ClientRegistrationResponse(clientId, "none", redirectUris);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    private static bool IsLoopbackRedirectUri(string? redirectUri)
    {
        if (string.IsNullOrEmpty(redirectUri) ||
            !Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal) &&
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
        {
            return false;
        }

        return string.IsNullOrEmpty(uri.Fragment) && uri.IsLoopback;
    }

    private async Task<IActionResult> ExchangeAuthorizationCodeAsync(
        string? code,
        string? redirectUri,
        string? clientId,
        string? codeVerifier)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(redirectUri) ||
            string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(codeVerifier))
        {
            return BadRequest(new OAuthError(
                "invalid_request", "code, redirect_uri, client_id and code_verifier are required."));
        }

        var data = authCodeService.Verify(code, clientId, redirectUri);
        if (data is null || !pkceService.Verify(codeVerifier, data.CodeChallenge))
        {
            return BadRequest(new OAuthError("invalid_grant", "The authorization code or PKCE verifier is invalid."));
        }

        return await IssueTokensAsync(data.UserId, clientId);
    }

    private async Task<IActionResult> ExchangeRefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest(new OAuthError("invalid_request", "refresh_token is required."));
        }

        var data = await refreshTokenService.VerifyAsync(refreshToken);
        if (data is null)
        {
            return BadRequest(new OAuthError("invalid_grant", "The refresh token is invalid or expired."));
        }

        return await IssueTokensAsync(data.UserId, data.ClientId);
    }

    private async Task<IActionResult> IssueTokensAsync(string userId, string clientId)
    {
        var accessToken = await authService.CreateAccessTokenAsync(userId, _oauthSettings.AccessTokenLifetimeMinutes);
        if (accessToken is null)
        {
            return BadRequest(new OAuthError("invalid_grant", "The user account no longer exists."));
        }

        var refreshToken = await refreshTokenService.IssueAsync(new RefreshTokenData(userId, clientId));
        var response = new TokenResponse(
            accessToken,
            "Bearer",
            _oauthSettings.AccessTokenLifetimeMinutes * 60,
            refreshToken,
            string.Join(' ', _oauthSettings.ScopesSupported));

        return Ok(response);
    }

    private RedirectResult RedirectWithError(string redirectUri, string state)
    {
        var separator = redirectUri.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        var location = $"{redirectUri}{separator}error=access_denied&state={Uri.EscapeDataString(state)}";

        return Redirect(location);
    }

    private string BaseUrl()
    {
        return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
    }
}
