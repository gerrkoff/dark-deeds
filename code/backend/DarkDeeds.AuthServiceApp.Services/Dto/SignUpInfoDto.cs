using System.ComponentModel.DataAnnotations;

namespace DarkDeeds.AuthServiceApp.Services.Dto
{
    public class SignUpInfoDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}