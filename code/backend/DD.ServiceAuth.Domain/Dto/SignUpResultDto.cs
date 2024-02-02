using DD.ServiceAuth.Domain.Enums;

namespace DD.ServiceAuth.Domain.Dto;

public class SignUpResultDto
{
    public string Token { get; set; }
    public SignUpResultEnum Result { get; set; }
}
