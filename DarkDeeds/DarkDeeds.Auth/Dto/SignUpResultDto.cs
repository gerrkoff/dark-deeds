using DarkDeeds.Auth.Enums;

namespace DarkDeeds.Auth.Dto
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
