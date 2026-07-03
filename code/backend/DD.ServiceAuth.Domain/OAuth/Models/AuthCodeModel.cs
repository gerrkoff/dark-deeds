namespace DD.ServiceAuth.Domain.OAuth.Models;

internal sealed record AuthCodeModel
{
    public required string UserId { get; init; }

    public required string ClientId { get; init; }

    public required string RedirectUri { get; init; }

    public required string CodeChallenge { get; init; }
}
