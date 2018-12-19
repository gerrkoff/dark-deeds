using DarkDeeds.Enums;

namespace DarkDeeds.Models.Account
{
    public class SignUpResultDto
    {
        public string Token { get; set; }
        public SignUpResultEnum Result { get; set; }
    }
}
