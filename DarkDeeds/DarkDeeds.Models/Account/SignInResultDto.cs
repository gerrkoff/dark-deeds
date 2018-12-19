using DarkDeeds.Enums;

namespace DarkDeeds.Models.Account
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}
