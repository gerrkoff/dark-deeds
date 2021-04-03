using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Enums;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}
