using DarkDeeds.Enums;

namespace DarkDeeds.Models.Account
{
    public class RegisterResultDto
    {
        public string Token { get; set; }
        public RegisterResultEnum Result { get; set; }
    }
}
