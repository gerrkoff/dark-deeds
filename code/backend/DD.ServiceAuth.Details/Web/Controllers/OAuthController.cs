using System.Diagnostics.CodeAnalysis;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.ServiceAuth.Domain.OAuth.Models;
using DD.ServiceAuth.Domain.OAuth.Services;
using DD.Shared.Details.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceAuth.Details.Web.Controllers;

[AllowAnonymous]
[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect_uri must be preserved and compared as an exact string, not URI-normalized.")]
public sealed class OAuthController(
    IOAuthFlowService oauthFlowService,
    IOAuthUrlService oauthUrlService,
    IUserAuth userAuth)
    : ControllerBase
{
    [HttpGet("/.well-known/oauth-authorization-server")]
    public IActionResult Metadata()
    {
        return Ok(oauthFlowService.BuildMetadata());
    }

    [HttpGet("/authorize")]
    public IActionResult Authorize(
        [FromQuery(Name = "response_type")] string? responseType,
        [FromQuery(Name = "client_id")] string? clientId,
        [FromQuery(Name = "redirect_uri")] string? redirectUri,
        [FromQuery(Name = "code_challenge")] string? codeChallenge,
        [FromQuery(Name = "code_challenge_method")] string? codeChallengeMethod,
        [FromQuery(Name = "state")] string? state)
    {
        if (!oauthUrlService.IsAllowedRedirectUri(redirectUri))
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

        // Same-origin relative redirect into the SPA (served from wwwroot at "/"). Kept relative so
        // the consent page stays on the host the client entered; this requires the SPA and the OAuth
        // endpoints to share an origin, which holds in the deployed monolith but not in the dev split
        // (SPA on Vite :3000, backend/Swagger on :5000).
        return Redirect($"/{Request.QueryString.Value}");
    }

    [HttpPost("/authorize")]
    public async Task<IActionResult> AuthorizeConsent([FromBody] OAuthAuthorizeRequestDto? request)
    {
        if (request is null || !oauthUrlService.IsAllowedRedirectUri(request.RedirectUri))
        {
            return BadRequest(OAuthErrorDto.RedirectUriNotLoopback);
        }

        if (string.IsNullOrEmpty(request.ClientId))
        {
            return BadRequest(OAuthErrorDto.ClientIdRequired);
        }

        if (string.IsNullOrEmpty(request.CodeChallenge))
        {
            return BadRequest(OAuthErrorDto.PkceChallengeRequired);
        }

        if (string.IsNullOrEmpty(request.State))
        {
            return BadRequest(OAuthErrorDto.StateRequired);
        }

        if (!string.Equals(request.Action, OAuthConstants.ActionAllow, StringComparison.Ordinal))
        {
            var denyRedirect = oauthUrlService.BuildErrorRedirect(
                request.RedirectUri, OAuthConstants.AccessDeniedError, request.State);

            return Ok(new OAuthRedirectResponseDto(denyRedirect));
        }

        if (!userAuth.IsAuthenticated())
        {
            return Unauthorized();
        }

        var location = await oauthFlowService.BuildAuthorizeRedirectAsync(
            OAuthConstants.ActionAllow,
            userAuth.UserId(),
            request.ClientId,
            request.RedirectUri,
            request.CodeChallenge,
            request.State);

        return Ok(new OAuthRedirectResponseDto(location));
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
        if (redirectUris is null || redirectUris.Count == 0 || redirectUris.Any(uri => !oauthUrlService.IsAllowedRedirectUri(uri)))
        {
            return BadRequest(OAuthErrorDto.RegistrationRedirectUrisNotLoopback);
        }

        var response = oauthFlowService.Register(redirectUris);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    private IActionResult TokenResult(OAuthResult<TokenResponseDto> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}
