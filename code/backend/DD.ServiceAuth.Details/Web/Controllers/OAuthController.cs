using System.Diagnostics.CodeAnalysis;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.ServiceAuth.Domain.OAuth.Models;
using DD.ServiceAuth.Domain.OAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceAuth.Details.Web.Controllers;

[AllowAnonymous]
[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect_uri must be preserved and compared as an exact string, not URI-normalized.")]
public sealed class OAuthController(IOAuthFlowService oauthFlowService)
    : ControllerBase
{
    [HttpGet("/.well-known/oauth-authorization-server")]
    public IActionResult Metadata()
    {
        var issuerBaseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        return Ok(oauthFlowService.BuildMetadata(issuerBaseUrl));
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
        if (!IsAllowedRedirectUri(redirectUri))
        {
            return BadRequest(OAuthErrorDto.RedirectUriNotLoopback);
        }

        if (string.IsNullOrEmpty(clientId))
        {
            return BadRequest(OAuthErrorDto.ClientIdRequired);
        }

        if (!string.Equals(responseType, OAuthConstants.ResponseTypeCode, StringComparison.Ordinal))
        {
            return BadRequest(OAuthErrorDto.UnsupportedResponseType);
        }

        if (string.IsNullOrEmpty(codeChallenge) ||
            !string.Equals(codeChallengeMethod, OAuthConstants.CodeChallengeMethodS256, StringComparison.Ordinal))
        {
            return BadRequest(OAuthErrorDto.PkceChallengeRequired);
        }

        if (string.IsNullOrEmpty(state))
        {
            return BadRequest(OAuthErrorDto.StateRequired);
        }

        var html = oauthFlowService.RenderConsentPage(
            clientId, redirectUri, codeChallenge, state, scope ?? string.Empty);

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
        if (!IsAllowedRedirectUri(redirectUri))
        {
            return BadRequest(OAuthErrorDto.RedirectUriNotLoopback);
        }

        if (string.IsNullOrEmpty(clientId))
        {
            return BadRequest(OAuthErrorDto.ClientIdRequired);
        }

        if (string.IsNullOrEmpty(codeChallenge) || string.IsNullOrEmpty(state))
        {
            return BadRequest(OAuthErrorDto.CodeChallengeAndStateRequired);
        }

        if (string.IsNullOrEmpty(username))
        {
            return BadRequest(OAuthErrorDto.UsernameRequired);
        }

        if (string.IsNullOrEmpty(password))
        {
            return BadRequest(OAuthErrorDto.PasswordRequired);
        }

        var location = await oauthFlowService.AuthorizeAsync(
            action ?? string.Empty,
            username,
            password,
            clientId,
            redirectUri,
            codeChallenge,
            state);

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

        if (string.Equals(grantType, OAuthConstants.GrantAuthorizationCode, StringComparison.Ordinal))
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(redirectUri) ||
                string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(codeVerifier))
            {
                return BadRequest(OAuthErrorDto.AuthorizationCodeFieldsRequired);
            }

            var result = await oauthFlowService.ExchangeAuthorizationCodeAsync(
                code, redirectUri, clientId, codeVerifier);

            return TokenResult(result);
        }

        if (string.Equals(grantType, OAuthConstants.GrantRefreshToken, StringComparison.Ordinal))
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(OAuthErrorDto.RefreshTokenRequired);
            }

            var result = await oauthFlowService.ExchangeRefreshTokenAsync(refreshToken);

            return TokenResult(result);
        }

        return BadRequest(OAuthErrorDto.UnsupportedGrantType);
    }

    [HttpPost("/register")]
    public IActionResult Register([FromBody] ClientRegistrationRequestDto? request)
    {
        var redirectUris = request?.RedirectUris;
        if (redirectUris is null || redirectUris.Count == 0 || redirectUris.Any(uri => !IsAllowedRedirectUri(uri)))
        {
            return BadRequest(OAuthErrorDto.RegistrationRedirectUrisNotLoopback);
        }

        var response = oauthFlowService.Register(redirectUris);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    private static bool IsAllowedRedirectUri([NotNullWhen(true)] string? redirectUri)
    {
        if (string.IsNullOrEmpty(redirectUri) ||
            !Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // VS Code registers well-known Microsoft redirect helpers (stable + Insiders) alongside its
        // loopback addresses, and dynamic client registration sends the whole redirect_uris set at
        // once, so allow these exact URIs; otherwise registration and the browser consent flow fail.
        // PKCE still protects the code exchange for these non-loopback redirects.
        string[] allowedExternalRedirectUris =
        [
            "https://vscode.dev/redirect",
            "https://insiders.vscode.dev/redirect",
        ];

        if (allowedExternalRedirectUris.Contains(redirectUri, StringComparer.Ordinal))
        {
            return true;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal) &&
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
        {
            return false;
        }

        return string.IsNullOrEmpty(uri.Fragment) && uri.IsLoopback;
    }

    private IActionResult TokenResult(OAuthResult<TokenResponseDto> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}
