using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.OAuth.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace DD.Tests.Integration;

public sealed class OAuthIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task Metadata_AuthorizationServer_ReturnsExpectedFields()
    {
        using var client = await CreateClientAsync();

        using var response = await client.GetAsync(
            new Uri(".well-known/oauth-authorization-server", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var metadata = await response.Content.ReadFromJsonAsync<AuthServerMetadataDto>();
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
        var metadata = await response.Content.ReadFromJsonAsync<ProtectedResourceMetadataDto>();
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
            new ClientRegistrationRequestDto([OAuthMcpTestSession.CallbackUri]));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var registration = await response.Content.ReadFromJsonAsync<ClientRegistrationResponseDto>();
        Assert.NotNull(registration);
        Assert.False(string.IsNullOrEmpty(registration.ClientId));
        Assert.Equal("none", registration.TokenEndpointAuthMethod);
        Assert.Contains(OAuthMcpTestSession.CallbackUri, registration.RedirectUris);
    }

    [Fact]
    public async Task Authorize_Get_Returns302RedirectToSpaWithQueryParameters()
    {
        using var oauthClient = await IntegrationEnvironmentLifetime.CreateNoRedirectClientAsync();

        var verifier = OAuthMcpHelper.GenerateCodeVerifier();
        var challenge = OAuthMcpHelper.ComputeS256Challenge(verifier);
        var state = Guid.NewGuid().ToString("N");
        var clientId = await OAuthMcpHelper.RegisterClientAsync(
            oauthClient, OAuthMcpTestSession.CallbackUri);

        var authorizeUri = OAuthMcpHelper.BuildAuthorizeUri(
            clientId, challenge, state, OAuthMcpTestSession.CallbackUri);

        using var response = await oauthClient.GetAsync(authorizeUri);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location;
        Assert.NotNull(location);
        var expectedLocation = new Uri(
            $"/?response_type={OAuthConstants.ResponseTypeCode}" +
            $"&client_id={Uri.EscapeDataString(clientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(OAuthMcpTestSession.CallbackUri)}" +
            $"&code_challenge={Uri.EscapeDataString(challenge)}" +
            $"&code_challenge_method={OAuthConstants.CodeChallengeMethodS256}" +
            $"&state={Uri.EscapeDataString(state)}",
            UriKind.Relative);
        Assert.Equal(expectedLocation, location);
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
            new Uri("/authorize", UriKind.Relative), request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<OAuthRedirectResponseDto>();
        Assert.NotNull(result);
        Assert.NotNull(result.RedirectUrl);
        var redirect = new Uri(result.RedirectUrl, UriKind.Absolute);
        AssertCallbackDestination(redirect, OAuthMcpTestSession.CallbackUri);
        var query = QueryHelpers.ParseQuery(redirect.Query);
        Assert.Equal(2, query.Count);
        Assert.Equal(OAuthConstants.AccessDeniedError, Assert.Single(query["error"]));
        Assert.Equal(state, Assert.Single(query["state"]));
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
            new Uri("/authorize", UriKind.Relative), request);

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

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(session.AccessToken);
        var issuedAt = long.Parse(
            jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Iat).Value,
            CultureInfo.InvariantCulture);
        var expiresAt = long.Parse(
            jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Exp).Value,
            CultureInfo.InvariantCulture);
        Assert.Equal(3600, expiresAt - issuedAt);
    }

    [Fact]
    public async Task Token_WrongCodeVerifier_Returns400InvalidGrant()
    {
        await using var user = await CreateUserClientAsync();
        using var oauthClient = await IntegrationEnvironmentLifetime.CreateNoRedirectClientAsync();

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
        var error = await response.Content.ReadFromJsonAsync<OAuthErrorDto>();
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

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await using var mcpClient = await McpTestClient.ConnectAsync(
            refreshed.AccessToken,
            timeout.Token);
    }

    private static void AssertCallbackDestination(Uri actual, string expectedCallbackUri)
    {
        var expected = new Uri(expectedCallbackUri, UriKind.Absolute);
        Assert.Equal(expected.Scheme, actual.Scheme, ignoreCase: true);
        Assert.Equal(expected.Authority, actual.Authority, ignoreCase: true);
        Assert.Equal(expected.AbsolutePath, actual.AbsolutePath);
    }

    // Local DTO for reading protected-resource metadata without importing SDK implementation types.
    private sealed record ProtectedResourceMetadataDto(
        [property: JsonPropertyName("resource")] string? Resource,
        [property: JsonPropertyName("authorization_servers")] IReadOnlyList<string>? AuthorizationServers,
        [property: JsonPropertyName("scopes_supported")] IReadOnlyList<string>? ScopesSupported);
}
