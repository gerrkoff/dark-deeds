namespace DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp.Dto
{
    public class SignInResultDto
    {
        public string Token { get; set; }
        public SignInResultEnum Result { get; set; }
    }
}