namespace DD.Shared.Details.Abstractions.Models;

public class AuthToken : AuthTokenBuildInfo
{
    public required Guid TokenId { get; init; }

    public required DateTimeOffset Expires { get; init; }
}
