using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Enums;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
