namespace DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp.Dto
{
    public enum SignUpResultEnum
    {
        Unknown,
        Success,
        UsernameAlreadyExists,
        PasswordInsecure,
        InvalidUsername
    }
}