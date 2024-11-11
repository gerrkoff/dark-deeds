using System.ComponentModel.DataAnnotations;

namespace DD.ServiceAuth.Domain;

public class AuthSettings
{
    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required]
    public required string Key { get; init; }

    [Required]
    public int Lifetime { get; init; }
}
