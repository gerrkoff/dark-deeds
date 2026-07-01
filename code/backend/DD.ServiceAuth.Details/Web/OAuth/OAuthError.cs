using System.Text.Json.Serialization;

namespace DD.ServiceAuth.Details.Web.OAuth;

public sealed record OAuthError(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("error_description")] string ErrorDescription);
