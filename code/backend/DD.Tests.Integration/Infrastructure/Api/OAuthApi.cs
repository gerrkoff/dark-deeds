using System.Net.Http.Json;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;

namespace DD.Tests.Integration.Infrastructure.Api;

internal static class OAuthApi
{
    private static readonly Uri AuthorizationMetadataUri =
        new(".well-known/oauth-authorization-server", UriKind.Relative);
    private static readonly Uri AuthorizeUri = new("/authorize", UriKind.Relative);
    private static readonly Uri ProtectedResourceMetadataUri =
        new(".well-known/oauth-protected-resource/mcp", UriKind.Relative);
    private static readonly Uri RegisterUri = new("/register", UriKind.Relative);
    private static readonly Uri TokenUri = new("/token", UriKind.Relative);

    internal static Task<HttpResponseMessage> GetAuthorizationMetadataAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(AuthorizationMetadataUri, cancellationToken);
    }

    internal static Task<HttpResponseMessage> GetProtectedResourceMetadataAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(ProtectedResourceMetadataUri, cancellationToken);
    }

    internal static Task<HttpResponseMessage> RegisterClientAsync(
        HttpClient client,
        string callbackUri,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(
            RegisterUri,
            new ClientRegistrationRequestDto([callbackUri]),
            cancellationToken);
    }

    internal static Task<HttpResponseMessage> GetAuthorizeAsync(
        HttpClient client,
        string clientId,
        string codeChallenge,
        string state,
        string callbackUri,
        CancellationToken cancellationToken = default)
    {
        var query = "response_type=code" +
            $"&client_id={Uri.EscapeDataString(clientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(callbackUri)}" +
            $"&code_challenge={Uri.EscapeDataString(codeChallenge)}" +
            $"&code_challenge_method={OAuthConstants.CodeChallengeMethodS256}" +
            $"&state={Uri.EscapeDataString(state)}";
        return client.GetAsync(
            new Uri($"{AuthorizeUri}?{query}", UriKind.Relative),
            cancellationToken);
    }

    internal static Task<HttpResponseMessage> AuthorizeAsync(
        HttpClient client,
        OAuthAuthorizeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(AuthorizeUri, request, cancellationToken);
    }

    internal static Task<HttpResponseMessage> ExchangeCodeAsync(
        HttpClient client,
        string code,
        string redirectUri,
        string clientId,
        string codeVerifier,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsync(
            TokenUri,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = OAuthConstants.GrantAuthorizationCode,
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["client_id"] = clientId,
                ["code_verifier"] = codeVerifier,
            }),
            cancellationToken);
    }

    internal static Task<HttpResponseMessage> ExchangeRefreshTokenAsync(
        HttpClient client,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsync(
            TokenUri,
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = OAuthConstants.GrantRefreshToken,
                ["refresh_token"] = refreshToken,
            }),
            cancellationToken);
    }
}
