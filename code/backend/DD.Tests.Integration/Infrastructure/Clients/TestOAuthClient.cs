using System.Net.Http.Headers;
using System.Net.Http.Json;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.Tests.Integration.Helpers;

namespace DD.Tests.Integration.Infrastructure.Clients;

public sealed class TestOAuthClient : IAsyncDisposable
{
    public const string CallbackUri = "http://127.0.0.1:43123/callback";

    private TestOAuthClient(
        HttpClient httpClient,
        TokenResponseDto tokens,
        bool codeExchangeCacheControlNoStore)
    {
        HttpClient = httpClient;
        Tokens = tokens;
        CodeExchangeCacheControlNoStore = codeExchangeCacheControlNoStore;
    }

    public HttpClient HttpClient { get; }

    public TokenResponseDto Tokens { get; private set; }

    public bool CodeExchangeCacheControlNoStore { get; }

    public static async Task<TestOAuthClient> CreateAsync(
        TestUserClient user,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var httpClient = await IntegrationEnvironmentLifetime.CreateClientAsync();
        try
        {
            var codeVerifier = OAuthHelper.GenerateCodeVerifier();
            var codeChallenge = OAuthHelper.ComputeS256Challenge(codeVerifier);
            var state = Guid.NewGuid().ToString("N");
            var clientId = await OAuthHelper.RegisterClientAsync(
                httpClient,
                CallbackUri,
                cancellationToken);

            var (authorizationCode, callbackState) = await OAuthHelper.ConsentAllowAsync(
                user.HttpClient,
                clientId,
                codeChallenge,
                state,
                CallbackUri,
                cancellationToken);
            if (!string.Equals(callbackState, state, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"OAuth callback state mismatch: expected '{state}', got '{callbackState}'.");
            }

            using var response = await OAuthHelper.ExchangeCodeRawAsync(
                httpClient,
                authorizationCode,
                CallbackUri,
                clientId,
                codeVerifier,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            var tokens = await response.Content.ReadFromJsonAsync<TokenResponseDto>(
                cancellationToken)
                ?? throw new InvalidOperationException(
                    "Token exchange returned an empty response.");
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            return new TestOAuthClient(
                httpClient,
                tokens,
                response.Headers.CacheControl?.NoStore == true);
        }
        catch
        {
            httpClient.Dispose();
            throw;
        }
    }

    public async Task<TokenResponseDto> RefreshAsync(
        CancellationToken cancellationToken = default)
    {
        Tokens = await OAuthHelper.ExchangeRefreshTokenAsync(
            HttpClient,
            Tokens.RefreshToken,
            cancellationToken);
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Tokens.AccessToken);
        return Tokens;
    }

    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
