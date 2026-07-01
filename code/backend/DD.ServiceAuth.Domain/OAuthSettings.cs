using System.ComponentModel.DataAnnotations;

namespace DD.ServiceAuth.Domain;

public class OAuthSettings
{
    [Range(1, int.MaxValue)]
    public int AccessTokenLifetimeMinutes { get; init; }

    [Range(1, int.MaxValue)]
    public int RefreshTokenLifetimeDays { get; init; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<string> ScopesSupported { get; init; }
}
