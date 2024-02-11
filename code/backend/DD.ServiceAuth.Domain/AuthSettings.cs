namespace DD.ServiceAuth.Domain;

public class AuthSettings
{
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public string? Key { get; init; }
    public int Lifetime { get; init; }
}
