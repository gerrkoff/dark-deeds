using DD.ServiceAuth.Domain.Enums;

namespace DD.ServiceAuth.Domain.Dto;

public class SignInResultDto
{
    public string Token { get; init; } = string.Empty;
    public SignInResult Result { get; init; }
}
