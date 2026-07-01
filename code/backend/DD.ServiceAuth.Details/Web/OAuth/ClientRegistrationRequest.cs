using System.Text.Json.Serialization;

namespace DD.ServiceAuth.Details.Web.OAuth;

public sealed record ClientRegistrationRequest(
    [property: JsonPropertyName("redirect_uris")] IReadOnlyList<string>? RedirectUris);
