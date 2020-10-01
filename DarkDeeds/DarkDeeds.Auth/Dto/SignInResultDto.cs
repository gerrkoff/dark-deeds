using DarkDeeds.Enums;

namespace DarkDeeds.Auth.Dto
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}
