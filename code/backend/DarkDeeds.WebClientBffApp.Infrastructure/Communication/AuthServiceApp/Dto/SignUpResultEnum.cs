namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto
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