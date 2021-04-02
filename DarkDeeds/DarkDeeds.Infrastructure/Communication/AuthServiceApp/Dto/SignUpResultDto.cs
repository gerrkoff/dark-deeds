using DarkDeeds.Infrastructure.Communication.AuthServiceApp.Enums;

namespace DarkDeeds.Infrastructure.Communication.AuthServiceApp.Dto
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
