namespace DD.ServiceAuth.Details.Web;

public class CurrentUserDto
{
    public string? Username { get; init; }

    public DateTimeOffset? Expires { get; init; }

    public bool UserAuthenticated { get; init; }
}
