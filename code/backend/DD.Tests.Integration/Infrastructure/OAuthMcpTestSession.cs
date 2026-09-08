using System.Net.Http.Json;
using System.Text.Json;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.Tests.Integration.Helpers;

namespace DD.Tests.Integration.Infrastructure;

public sealed class OAuthMcpTestSession : IAsyncDisposable
{
    public const string CallbackUri = "http://127.0.0.1:43123/callback";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly TestUserClient _user;

    private readonly HttpClient _oauthClient;

    private OAuthMcpTestSession(
        TestUserClient user,
        HttpClient oauthClient,
        string clientId,
        string codeVerifier,
        string codeChallenge,
        string state)
    {
        _user = user;
        _oauthClient = oauthClient;
        ClientId = clientId;
        CodeVerifier = codeVerifier;
        CodeChallenge = codeChallenge;
        State = state;
    }

    public string LoginToken => _user.Token;

    public string ClientId { get; }

    public string CodeVerifier { get; }

    public string CodeChallenge { get; }

    public string State { get; }

    public string? AccessToken { get; private set; }

    public string? RefreshToken { get; private set; }

    // Captured from the authorization-code exchange response.
    public TokenResponseDto? LastTokenResponse { get; private set; }

    public bool ExchangeCacheControlNoStore { get; private set; }

    // Creates a fully-initialized session: registers a dynamic client, generates an RFC 7636
    // S256 PKCE pair, submits authenticated allow consent, validates the callback state,
    // exchanges the authorization code, and stores the resulting login/access/refresh token state.
    public static async Task<OAuthMcpTestSession> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var oauthClient = await IntegrationEnvironmentLifetime.CreateOAuthClientAsync();
        TestUserClient? user = null;
        var shouldDispose = true;

        try
        {
            user = await TestUserClient.CreateAsync(cancellationToken);

            var codeVerifier = OAuthMcpHelper.GenerateCodeVerifier();
            var codeChallenge = OAuthMcpHelper.ComputeS256Challenge(codeVerifier);
            var state = Guid.NewGuid().ToString("N");
            var clientId = await OAuthMcpHelper.RegisterClientAsync(
                oauthClient, CallbackUri, cancellationToken);

            var session = new OAuthMcpTestSession(
                user, oauthClient, clientId, codeVerifier, codeChallenge, state);

            var (code, callbackState) = await OAuthMcpHelper.ConsentAllowAsync(
                user.HttpClient, clientId, codeChallenge, state, CallbackUri, cancellationToken);

            if (!string.Equals(callbackState, state, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"OAuth callback state mismatch: expected '{state}', got '{callbackState}'.");
            }

            await session.ExchangeCodeInternalAsync(code, codeVerifier, cancellationToken);

            shouldDispose = false;
            return session;
        }
        finally
        {
            if (shouldDispose)
            {
                if (user is not null)
                {
                    await user.DisposeAsync();
                }

                oauthClient.Dispose();
            }
        }
    }

    public Task<TokenResponseDto> ExchangeRefreshAsync(CancellationToken cancellationToken = default)
    {
        if (RefreshToken is null)
        {
            throw new InvalidOperationException("No refresh token is available on this session.");
        }

        return OAuthMcpHelper.ExchangeRefreshTokenAsync(_oauthClient, RefreshToken, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        _oauthClient.Dispose();
        return _user.DisposeAsync();
    }

    private async Task ExchangeCodeInternalAsync(
        string code,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        using var response = await OAuthMcpHelper.ExchangeCodeRawAsync(
            _oauthClient, code, CallbackUri, ClientId, codeVerifier, cancellationToken);
        response.EnsureSuccessStatusCode();

        ExchangeCacheControlNoStore = response.Headers.CacheControl?.NoStore == true;
        LastTokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>(
            JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Token exchange returned an empty response.");
        AccessToken = LastTokenResponse.AccessToken;
        RefreshToken = LastTokenResponse.RefreshToken;
    }
}
