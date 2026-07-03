using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DD.ServiceAuth.Domain.OAuth;

internal sealed class OAuthSettings
{
    [Range(1, int.MaxValue)]
    public int AccessTokenLifetimeMinutes { get; init; }

    [Range(1, int.MaxValue)]
    public int RefreshTokenLifetimeDays { get; init; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<string> ScopesSupported { get; init; }

    [Required]
    [SuppressMessage(
        "Design",
        "CA1056:URI-like properties should not be strings",
        Justification = "OAuth issuer base URL is emitted verbatim in discovery metadata and must not be URI-normalized.")]
    public required string IssuerBaseUrl { get; init; }
}
