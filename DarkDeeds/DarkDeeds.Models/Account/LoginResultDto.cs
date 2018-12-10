using DarkDeeds.Enums;

namespace DarkDeeds.Models.Account
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public LoginResultEnum Result { get; set; }
    }
}
