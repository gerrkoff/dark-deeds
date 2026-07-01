using System.Diagnostics.CodeAnalysis;

namespace DD.ServiceAuth.Domain.Services;

public sealed record AuthCodeData
{
    public required string UserId { get; init; }

    public required string ClientId { get; init; }

    [SuppressMessage(
        "Design",
        "CA1056:URI-like properties should not be strings",
        Justification = "OAuth redirect_uri must be compared by exact string match and is embedded verbatim in the signed authorization code.")]
    public required string RedirectUri { get; init; }

    public required string CodeChallenge { get; init; }
}
