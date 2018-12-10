using System.ComponentModel.DataAnnotations;

namespace DarkDeeds.Models.Account
{
    public class LoginInfoDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}