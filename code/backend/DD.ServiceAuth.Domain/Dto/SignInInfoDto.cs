using System.ComponentModel.DataAnnotations;

namespace DD.ServiceAuth.Domain.Dto;

public class SignInInfoDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
