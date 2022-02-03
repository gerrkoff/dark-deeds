using DarkDeeds.ServiceAuth.Services.Enums;

namespace DarkDeeds.ServiceAuth.Services.Dto
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}
