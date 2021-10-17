using DarkDeeds.AuthServiceApp.Services.Enums;

namespace DarkDeeds.AuthServiceApp.Services.Dto
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}
