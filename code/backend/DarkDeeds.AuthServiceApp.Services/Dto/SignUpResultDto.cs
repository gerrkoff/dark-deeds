using DarkDeeds.AuthServiceApp.Services.Enums;

namespace DarkDeeds.AuthServiceApp.Services.Dto
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
