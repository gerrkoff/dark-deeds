using System.Text.Json.Serialization;
using DD.ServiceAuth.Domain.OAuth.Dto;
using Microsoft.AspNetCore.WebUtilities;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class OAuthReader
{
    internal static Task<AuthServerMetadataDto> ReadAuthorizationMetadataAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<AuthServerMetadataDto>(
            response,
            cancellationToken);
    }

    internal static Task<ProtectedResourceMetadataContract> ReadProtectedResourceMetadataAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<ProtectedResourceMetadataContract>(
            response,
            cancellationToken);
    }

    internal static Task<ClientRegistrationResponseDto> ReadClientRegistrationAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<ClientRegistrationResponseDto>(
            response,
            cancellationToken);
    }

    internal static Task<OAuthRedirectResponseDto> ReadRedirectAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<OAuthRedirectResponseDto>(
            response,
            cancellationToken);
    }

    internal static Task<TokenResponseDto> ReadTokenAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<TokenResponseDto>(
            response,
            cancellationToken);
    }

    internal static Task<OAuthErrorDto> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<OAuthErrorDto>(
            response,
            ensureSuccess: false,
            cancellationToken);
    }

    internal static (string Code, string State) ReadCallback(
        string redirectUrl,
        string expectedCallbackUri)
    {
        var uri = new Uri(redirectUrl, UriKind.Absolute);
        var expectedUri = new Uri(expectedCallbackUri, UriKind.Absolute);
        if (!string.Equals(uri.Scheme, expectedUri.Scheme, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.Authority, expectedUri.Authority, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.AbsolutePath, expectedUri.AbsolutePath, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Consent response redirected to an unexpected callback URI.");
        }

        var parameters = QueryHelpers.ParseQuery(uri.Query);
        if (!parameters.TryGetValue("code", out var codes) ||
            codes.Count != 1 ||
            string.IsNullOrEmpty(codes[0]))
        {
            throw new InvalidOperationException(
                "Callback redirect URL did not contain a 'code' parameter.");
        }

        if (!parameters.TryGetValue("state", out var states) ||
            states.Count != 1 ||
            string.IsNullOrEmpty(states[0]))
        {
            throw new InvalidOperationException(
                "Callback redirect URL did not contain a 'state' parameter.");
        }

        return (codes[0]!, states[0]!);
    }

    internal sealed record ProtectedResourceMetadataContract(
        [property: JsonPropertyName("resource")] string? Resource,
        [property: JsonPropertyName("authorization_servers")] IReadOnlyList<string>? AuthorizationServers,
        [property: JsonPropertyName("scopes_supported")] IReadOnlyList<string>? ScopesSupported);
}
