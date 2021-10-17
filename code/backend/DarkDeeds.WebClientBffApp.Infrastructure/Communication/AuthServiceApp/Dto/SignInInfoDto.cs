using System.ComponentModel.DataAnnotations;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto
{
    public class SignInInfoDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}