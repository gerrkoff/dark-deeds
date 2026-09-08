using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using Xunit;

namespace DD.Tests.Integration;

public sealed class OAuthIntegrationTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Metadata_AuthorizationServer_ReturnsExpectedFields()
    {
        using var client = await CreateClientAsync();

        using var response = await client.GetAsync(
            new Uri(".well-known/oauth-authorization-server", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var metadata = await response.Content.ReadFromJsonAsync<AuthServerMetadataDto>(JsonOptions);
        Assert.NotNull(metadata);
        Assert.Equal(DarkDeedsWebApplicationFactory.AuthIssuer, metadata.Issuer);
        Assert.Equal(
            $"{DarkDeedsWebApplicationFactory.AuthIssuer}/authorize",
            metadata.AuthorizationEndpoint);
        Assert.Equal(
            $"{DarkDeedsWebApplicationFactory.AuthIssuer}/token",
            metadata.TokenEndpoint);
        Assert.Equal(
            $"{DarkDeedsWebApplicationFactory.AuthIssuer}/register",
            metadata.RegistrationEndpoint);
        Assert.Equal(
            [OAuthConstants.ResponseTypeCode],
            metadata.ResponseTypesSupported);
        Assert.Equal(
            [OAuthConstants.GrantAuthorizationCode, OAuthConstants.GrantRefreshToken],
            metadata.GrantTypesSupported);
        Assert.Equal(
            [OAuthConstants.CodeChallengeMethodS256],
            metadata.CodeChallengeMethodsSupported);
        Assert.Equal(["none"], metadata.TokenEndpointAuthMethodsSupported);
        Assert.Equal(["mcp"], metadata.ScopesSupported);
    }

    [Fact]
    public async Task Metadata_ProtectedResource_ReturnsExpectedFields()
    {
        using var client = await CreateClientAsync();

        using var response = await client.GetAsync(
            new Uri(".well-known/oauth-protected-resource/mcp", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var metadata = await response.Content.ReadFromJsonAsync<ProtectedResourceMetadataDto>(JsonOptions);
        Assert.NotNull(metadata);
        Assert.Equal(
            $"{DarkDeedsWebApplicationFactory.AuthIssuer}/mcp",
            metadata.Resource);
        Assert.NotNull(metadata.AuthorizationServers);
        Assert.Equal(
            [DarkDeedsWebApplicationFactory.AuthIssuer],
            metadata.AuthorizationServers);
        Assert.NotNull(metadata.ScopesSupported);
        Assert.Equal(["mcp"], metadata.ScopesSupported);
    }

    [Fact]
    public async Task Register_ValidLoopbackRedirectUri_Returns201WithClientId()
    {
        using var client = await CreateClientAsync();

        using var response = await client.PostAsJsonAsync(
            new Uri("/register", UriKind.Relative),
            new ClientRegistrationRequestDto([OAuthMcpTestSession.CallbackUri]),
            JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var registration = await response.Content.ReadFromJsonAsync<ClientRegistrationResponseDto>(JsonOptions);
        Assert.NotNull(registration);
        Assert.False(string.IsNullOrEmpty(registration.ClientId));
        Assert.Equal("none", registration.TokenEndpointAuthMethod);
        Assert.Contains(OAuthMcpTestSession.CallbackUri, registration.RedirectUris);
    }

    [Fact]
    public async Task Authorize_Get_Returns302RedirectToSpaWithQueryParameters()
    {
        await using var user = await CreateUserClientAsync();
        using var oauthClient = await IntegrationEnvironmentLifetime.CreateOAuthClientAsync();

        var verifier = OAuthMcpHelper.GenerateCodeVerifier();
        var challenge = OAuthMcpHelper.ComputeS256Challenge(verifier);
        var state = Guid.NewGuid().ToString("N");
        var clientId = await OAuthMcpHelper.RegisterClientAsync(
            oauthClient, OAuthMcpTestSession.CallbackUri);

        var authorizeUri = OAuthMcpHelper.BuildAuthorizeUri(
            clientId, challenge, state, OAuthMcpTestSession.CallbackUri);

        using var response = await oauthClient.GetAsync(authorizeUri);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location?.ToString();
        Assert.NotNull(location);
        Assert.Contains("response_type=code", location, StringComparison.Ordinal);
        Assert.Contains($"client_id={Uri.EscapeDataString(clientId)}", location, StringComparison.Ordinal);
        Assert.Contains("code_challenge=", location, StringComparison.Ordinal);
        Assert.Contains($"state={Uri.EscapeDataString(state)}", location, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Authorize_DenyConsent_Returns200WithAccessDeniedAndPreservesState()
    {
        using var client = await CreateClientAsync();

        var verifier = OAuthMcpHelper.GenerateCodeVerifier();
        var challenge = OAuthMcpHelper.ComputeS256Challenge(verifier);
        var state = Guid.NewGuid().ToString("N");
        var clientId = await OAuthMcpHelper.RegisterClientAsync(
            client, OAuthMcpTestSession.CallbackUri);

        var request = new OAuthAuthorizeRequestDto(
            Action: "deny",
            ClientId: clientId,
            RedirectUri: OAuthMcpTestSession.CallbackUri,
            CodeChallenge: challenge,
            State: state);

        using var response = await client.PostAsJsonAsync(
            new Uri("/authorize", UriKind.Relative), request, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<OAuthRedirectResponseDto>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotNull(result.RedirectUrl);
        Assert.Contains("error=access_denied", result.RedirectUrl, StringComparison.Ordinal);
        Assert.Contains(
            $"state={Uri.EscapeDataString(state)}",
            result.RedirectUrl,
            StringComparison.Ordinal);
        Assert.DoesNotContain("code=", result.RedirectUrl, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Authorize_UnauthenticatedAllow_Returns401()
    {
        using var client = await CreateClientAsync();

        var verifier = OAuthMcpHelper.GenerateCodeVerifier();
        var challenge = OAuthMcpHelper.ComputeS256Challenge(verifier);
        var state = Guid.NewGuid().ToString("N");
        var clientId = await OAuthMcpHelper.RegisterClientAsync(
            client, OAuthMcpTestSession.CallbackUri);

        var request = new OAuthAuthorizeRequestDto(
            Action: OAuthConstants.ActionAllow,
            ClientId: clientId,
            RedirectUri: OAuthMcpTestSession.CallbackUri,
            CodeChallenge: challenge,
            State: state);

        using var response = await client.PostAsJsonAsync(
            new Uri("/authorize", UriKind.Relative), request, JsonOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Token_AuthorizationCodeExchange_ReturnsBearerWithMcpScopeAndNoStore()
    {
        await using var session = await OAuthMcpTestSession.CreateAsync();

        Assert.NotNull(session.LastTokenResponse);
        Assert.Equal("Bearer", session.LastTokenResponse.TokenType);
        Assert.Equal("mcp", session.LastTokenResponse.Scope);
        Assert.Equal(3600, session.LastTokenResponse.ExpiresIn);
        Assert.False(string.IsNullOrEmpty(session.AccessToken));
        Assert.False(string.IsNullOrEmpty(session.RefreshToken));
        Assert.True(session.ExchangeCacheControlNoStore);
    }

    [Fact]
    public async Task Token_WrongCodeVerifier_Returns400InvalidGrant()
    {
        await using var user = await CreateUserClientAsync();
        using var oauthClient = await IntegrationEnvironmentLifetime.CreateOAuthClientAsync();

        var verifier = OAuthMcpHelper.GenerateCodeVerifier();
        var challenge = OAuthMcpHelper.ComputeS256Challenge(verifier);
        var state = Guid.NewGuid().ToString("N");
        var clientId = await OAuthMcpHelper.RegisterClientAsync(
            oauthClient, OAuthMcpTestSession.CallbackUri);

        var (code, _) = await OAuthMcpHelper.ConsentAllowAsync(
            user.HttpClient, clientId, challenge, state, OAuthMcpTestSession.CallbackUri);

        using var response = await OAuthMcpHelper.ExchangeCodeRawAsync(
            oauthClient,
            code,
            OAuthMcpTestSession.CallbackUri,
            clientId,
            codeVerifier: "wrong-verifier-" + Guid.NewGuid().ToString("N"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<OAuthErrorDto>(JsonOptions);
        Assert.NotNull(error);
        Assert.Equal("invalid_grant", error.Error);
    }

    [Fact]
    public async Task Token_RefreshTokenExchange_ReturnsNewValidAccessToken()
    {
        await using var session = await OAuthMcpTestSession.CreateAsync();

        var refreshed = await session.ExchangeRefreshAsync();

        Assert.NotNull(refreshed);
        Assert.Equal("Bearer", refreshed.TokenType);
        Assert.False(string.IsNullOrEmpty(refreshed.AccessToken));
        Assert.NotEqual(session.AccessToken, refreshed.AccessToken);
    }

    // Local DTO for reading protected-resource metadata without importing SDK implementation types.
    private sealed record ProtectedResourceMetadataDto(
        [property: JsonPropertyName("resource")] string? Resource,
        [property: JsonPropertyName("authorization_servers")] IReadOnlyList<string>? AuthorizationServers,
        [property: JsonPropertyName("scopes_supported")] IReadOnlyList<string>? ScopesSupported);
}
