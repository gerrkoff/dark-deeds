using DarkDeeds.ServiceAuth.Services.Enums;

namespace DarkDeeds.ServiceAuth.Services.Dto
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
