using System.Diagnostics.CodeAnalysis;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.ServiceAuth.Domain.OAuth.Models;
using DD.ServiceAuth.Domain.Services;
using Microsoft.Extensions.Options;

namespace DD.ServiceAuth.Domain.OAuth.Services;

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth issuer/redirect URIs are exchanged as exact strings and must not be URI-normalized.")]
public interface IOAuthFlowService
{
    AuthServerMetadataDto BuildMetadata();

    Task<string> BuildAuthorizeRedirectAsync(
        string action,
        string userId,
        string clientId,
        string redirectUri,
        string codeChallenge,
        string state);

    Task<OAuthResult<TokenResponseDto>> ExchangeAuthorizationCodeAsync(
        string code,
        string redirectUri,
        string clientId,
        string codeVerifier);

    Task<OAuthResult<TokenResponseDto>> ExchangeRefreshTokenAsync(string refreshToken);

    ClientRegistrationResponseDto Register(IReadOnlyList<string> redirectUris);
}

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth issuer/redirect URIs are exchanged as exact strings and must not be URI-normalized.")]
internal sealed class OAuthFlowService(
    IAuthService authService,
    IAuthCodeService authCodeService,
    IPkceService pkceService,
    IRefreshTokenService refreshTokenService,
    IOAuthUrlService oauthUrlService,
    IOptions<OAuthSettings> oauthSettings)
    : IOAuthFlowService
{
    private readonly OAuthSettings _oauthSettings = oauthSettings.Value;

    public AuthServerMetadataDto BuildMetadata()
    {
        var issuerBaseUrl = _oauthSettings.IssuerBaseUrl;
        return new AuthServerMetadataDto(
            issuerBaseUrl,
            $"{issuerBaseUrl}/authorize",
            $"{issuerBaseUrl}/token",
            $"{issuerBaseUrl}/register",
            [OAuthConstants.ResponseTypeCode],
            [OAuthConstants.GrantAuthorizationCode, OAuthConstants.GrantRefreshToken],
            [OAuthConstants.CodeChallengeMethodS256],
            ["none"],
            _oauthSettings.ScopesSupported);
    }

    public async Task<string> BuildAuthorizeRedirectAsync(
        string action,
        string userId,
        string clientId,
        string redirectUri,
        string codeChallenge,
        string state)
    {
        // Defense in depth: only "allow" mints an authorization code; any other action yields
        // access_denied, even though the controller gates authentication and routes deny separately.
        if (!string.Equals(action, OAuthConstants.ActionAllow, StringComparison.Ordinal))
        {
            return oauthUrlService.BuildErrorRedirect(redirectUri, OAuthConstants.AccessDeniedError, state);
        }

        var code = await authCodeService.IssueAsync(new AuthCodeModel
        {
            UserId = userId,
            ClientId = clientId,
            RedirectUri = redirectUri,
            CodeChallenge = codeChallenge,
        });

        return oauthUrlService.BuildSuccessRedirect(redirectUri, code, state);
    }

    public async Task<OAuthResult<TokenResponseDto>> ExchangeAuthorizationCodeAsync(
        string code,
        string redirectUri,
        string clientId,
        string codeVerifier)
    {
        var data = await authCodeService.VerifyAsync(code, clientId, redirectUri);
        if (data is null || !pkceService.Verify(codeVerifier, data.CodeChallenge))
        {
            return OAuthResult<TokenResponseDto>.Failure(OAuthErrorDto.InvalidAuthorizationCode);
        }

        return await IssueTokensAsync(data.UserId, clientId);
    }

    public async Task<OAuthResult<TokenResponseDto>> ExchangeRefreshTokenAsync(string refreshToken)
    {
        var data = await refreshTokenService.VerifyAsync(refreshToken);
        if (data is null)
        {
            return OAuthResult<TokenResponseDto>.Failure(OAuthErrorDto.InvalidRefreshToken);
        }

        return await IssueTokensAsync(data.UserId, data.ClientId);
    }

    public ClientRegistrationResponseDto Register(IReadOnlyList<string> redirectUris)
    {
        var clientId = Guid.NewGuid().ToString("N");
        return new ClientRegistrationResponseDto(clientId, "none", redirectUris);
    }

    private async Task<OAuthResult<TokenResponseDto>> IssueTokensAsync(string userId, string clientId)
    {
        var accessToken = await authService.CreateAccessTokenAsync(userId, _oauthSettings.AccessTokenLifetimeMinutes);
        if (accessToken is null)
        {
            return OAuthResult<TokenResponseDto>.Failure(OAuthErrorDto.UserNotFound);
        }

        var refreshToken = await refreshTokenService.IssueAsync(new RefreshTokenModel(userId, clientId));
        var response = new TokenResponseDto(
            accessToken,
            "Bearer",
            _oauthSettings.AccessTokenLifetimeMinutes * 60,
            refreshToken,
            string.Join(' ', _oauthSettings.ScopesSupported));

        return OAuthResult<TokenResponseDto>.Success(response);
    }
}
