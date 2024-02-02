using DD.ServiceAuth.Domain.Enums;

namespace DD.ServiceAuth.Domain.Dto;

public class SignInResultDto
{
    public string Token { get; set; }
    public SignInResultEnum Result { get; set; }
}
