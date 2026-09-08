using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;

namespace DD.Tests.Integration.Helpers;

internal static class OAuthMcpHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    public static string ComputeS256Challenge(string codeVerifier)
    {
        var bytes = Encoding.ASCII.GetBytes(codeVerifier);
        return Base64UrlEncode(SHA256.HashData(bytes));
    }

    public static Uri BuildAuthorizeUri(
        string clientId,
        string codeChallenge,
        string state,
        string callbackUri)
    {
        var query = "response_type=code" +
            $"&client_id={Uri.EscapeDataString(clientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(callbackUri)}" +
            $"&code_challenge={Uri.EscapeDataString(codeChallenge)}" +
            "&code_challenge_method=S256" +
            $"&state={Uri.EscapeDataString(state)}";
        return new Uri($"/authorize?{query}", UriKind.Relative);
    }

    public static async Task<string> RegisterClientAsync(
        HttpClient client,
        string callbackUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(
            new Uri("/register", UriKind.Relative),
            new ClientRegistrationRequestDto([callbackUri]),
            JsonOptions,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ClientRegistrationResponseDto>(
            JsonOptions, cancellationToken);
        return result?.ClientId
            ?? throw new InvalidOperationException(
                "Dynamic client registration did not return a client_id.");
    }

    // Posts the allow consent to /authorize using an authenticated client and returns the
    // (code, state) pair parsed from the redirect URL in the response body.
    public static async Task<(string Code, string State)> ConsentAllowAsync(
        HttpClient authenticatedClient,
        string clientId,
        string codeChallenge,
        string state,
        string callbackUri,
        CancellationToken cancellationToken = default)
    {
        var request = new OAuthAuthorizeRequestDto(
            OAuthConstants.ActionAllow, clientId, callbackUri, codeChallenge, state);

        using var response = await authenticatedClient.PostAsJsonAsync(
            new Uri("/authorize", UriKind.Relative),
            request,
            JsonOptions,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OAuthRedirectResponseDto>(
            JsonOptions, cancellationToken);
        var redirectUrl = result?.RedirectUrl
            ?? throw new InvalidOperationException(
                "Consent response did not include a redirect URL.");

        return ParseCallbackRedirect(redirectUrl);
    }

    // Posts the authorization-code exchange and returns the raw response so callers can inspect
    // response headers (e.g. Cache-Control) before reading the body. Caller must dispose the
    // returned HttpResponseMessage.
    public static Task<HttpResponseMessage> ExchangeCodeRawAsync(
        HttpClient client,
        string code,
        string redirectUri,
        string clientId,
        string codeVerifier,
        CancellationToken cancellationToken = default)
    {
        var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = OAuthConstants.GrantAuthorizationCode,
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            ["client_id"] = clientId,
            ["code_verifier"] = codeVerifier,
        });

        return client.PostAsync(new Uri("/token", UriKind.Relative), formContent, cancellationToken);
    }

    public static async Task<TokenResponseDto> ExchangeRefreshTokenAsync(
        HttpClient client,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = OAuthConstants.GrantRefreshToken,
            ["refresh_token"] = refreshToken,
        });

        using var response = await client.PostAsync(
            new Uri("/token", UriKind.Relative), formContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenResponseDto>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Refresh token exchange returned an empty response.");
    }

    public static (string Code, string State) ParseCallbackRedirect(string redirectUrl)
    {
        var uri = new Uri(redirectUrl, UriKind.Absolute);
        var parameters = uri.Query
            .TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => Uri.UnescapeDataString(parts[0]),
                parts => Uri.UnescapeDataString(parts[1]),
                StringComparer.Ordinal);

        if (!parameters.TryGetValue("code", out var code))
        {
            throw new InvalidOperationException(
                "Callback redirect URL did not contain a 'code' parameter.");
        }

        if (!parameters.TryGetValue("state", out var state))
        {
            throw new InvalidOperationException(
                "Callback redirect URL did not contain a 'state' parameter.");
        }

        return (code, state);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
