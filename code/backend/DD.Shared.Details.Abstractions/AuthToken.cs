namespace DD.Shared.Details.Abstractions;

public class AuthToken
{
    public string UserId { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public DateTime? Expires { get; init; }
}
