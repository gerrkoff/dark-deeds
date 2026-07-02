using System.Text.Json.Serialization;

namespace DD.ServiceAuth.Domain.OAuth.Dto;

public sealed record ClientRegistrationRequestDto(
    [property: JsonPropertyName("redirect_uris")] IReadOnlyList<string>? RedirectUris);
