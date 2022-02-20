using DarkDeeds.ServiceAuth.Dto.Enums;

namespace DarkDeeds.ServiceAuth.Dto.Dto
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
