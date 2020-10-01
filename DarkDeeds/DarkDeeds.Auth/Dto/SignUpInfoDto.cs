using System.ComponentModel.DataAnnotations;

namespace DarkDeeds.Auth.Dto
{
    public class SignUpInfoDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}