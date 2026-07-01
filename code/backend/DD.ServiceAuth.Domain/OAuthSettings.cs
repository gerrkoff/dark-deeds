using System.ComponentModel.DataAnnotations;

namespace DD.ServiceAuth.Domain;

public class OAuthSettings
{
    [Required]
    public int AccessTokenLifetimeMinutes { get; init; }

    [Required]
    public int RefreshTokenLifetimeDays { get; init; }

    [Required]
    public required IReadOnlyList<string> ScopesSupported { get; init; }
}
