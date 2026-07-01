using System.Text.Json.Serialization;

namespace DD.ServiceAuth.Details.Web.OAuth;

public sealed record AuthServerMetadata(
    [property: JsonPropertyName("issuer")] string Issuer,
    [property: JsonPropertyName("authorization_endpoint")] string AuthorizationEndpoint,
    [property: JsonPropertyName("token_endpoint")] string TokenEndpoint,
    [property: JsonPropertyName("registration_endpoint")] string RegistrationEndpoint,
    [property: JsonPropertyName("response_types_supported")] IReadOnlyList<string> ResponseTypesSupported,
    [property: JsonPropertyName("grant_types_supported")] IReadOnlyList<string> GrantTypesSupported,
    [property: JsonPropertyName("code_challenge_methods_supported")] IReadOnlyList<string> CodeChallengeMethodsSupported,
    [property: JsonPropertyName("token_endpoint_auth_methods_supported")] IReadOnlyList<string> TokenEndpointAuthMethodsSupported,
    [property: JsonPropertyName("scopes_supported")] IReadOnlyList<string> ScopesSupported);
