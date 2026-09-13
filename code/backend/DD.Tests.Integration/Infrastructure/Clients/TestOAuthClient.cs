using System.Net.Http.Headers;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;

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
            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.ComputeS256Challenge(codeVerifier);
            var state = Guid.NewGuid().ToString("N");
            using var registrationResponse = await OAuthApi.RegisterClientAsync(
                httpClient,
                CallbackUri,
                cancellationToken);
            var registration = await OAuthReader.ReadClientRegistrationAsync(
                registrationResponse,
                cancellationToken);
            if (string.IsNullOrWhiteSpace(registration.ClientId))
            {
                throw new InvalidOperationException(
                    "Dynamic client registration did not return a client_id.");
            }

            using var authorizeResponse = await OAuthApi.AuthorizeAsync(
                user.HttpClient,
                new OAuthAuthorizeRequestDto(
                    OAuthConstants.ActionAllow,
                    registration.ClientId,
                    CallbackUri,
                    codeChallenge,
                    state),
                cancellationToken);
            var redirect = await OAuthReader.ReadRedirectAsync(
                authorizeResponse,
                cancellationToken);
            var redirectUrl = redirect.RedirectUrl
                ?? throw new InvalidOperationException(
                    "Consent response did not include a redirect URL.");
            var (authorizationCode, callbackState) = OAuthReader.ReadCallback(
                redirectUrl,
                CallbackUri);
            if (!string.Equals(callbackState, state, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"OAuth callback state mismatch: expected '{state}', got '{callbackState}'.");
            }

            using var response = await OAuthApi.ExchangeCodeAsync(
                httpClient,
                authorizationCode,
                CallbackUri,
                registration.ClientId,
                codeVerifier,
                cancellationToken);

            var tokens = await OAuthReader.ReadTokenAsync(response, cancellationToken);
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
        using var response = await OAuthApi.ExchangeRefreshTokenAsync(
            HttpClient,
            Tokens.RefreshToken,
            cancellationToken);
        Tokens = await OAuthReader.ReadTokenAsync(response, cancellationToken);
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
