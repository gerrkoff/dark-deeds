using DarkDeeds.ServiceAuth.Dto.Enums;

namespace DarkDeeds.ServiceAuth.Dto.Dto
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}
