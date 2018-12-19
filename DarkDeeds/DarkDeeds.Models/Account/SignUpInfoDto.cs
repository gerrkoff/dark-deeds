using System.ComponentModel.DataAnnotations;

namespace DarkDeeds.Models.Account
{
    public class SignUpInfoDto
    {
        [Required]
        [MaxLength(30)]
        [RegularExpression("[a-zA-Z0-9_]+")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}