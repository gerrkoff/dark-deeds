using System.Text.Json.Serialization;

namespace DD.ServiceAuth.Details.Web.OAuth;

public sealed record ClientRegistrationResponse(
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("token_endpoint_auth_method")] string TokenEndpointAuthMethod,
    [property: JsonPropertyName("redirect_uris")] IReadOnlyList<string> RedirectUris);
