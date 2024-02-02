using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Enums;

namespace DD.ServiceAuth.Domain.Services;

public interface ITestService
{
    Task<TestUserDto> CreateTestUserAsync();
}

class TestService(IAuthService authService) : ITestService
{
    public async Task<TestUserDto> CreateTestUserAsync()
    {
        var signUpInfo = new SignUpInfoDto
        {
            Username = Guid.NewGuid().ToString(),
            Password = "QWERTY123456qwerty!@#$%^",
        };

        var result = await authService.SignUpAsync(signUpInfo);

        if (result.Result != SignUpResultEnum.Success)
            throw new InvalidOperationException();

        var userId = await authService.GetUserIdAsync(signUpInfo.Username);

        return new TestUserDto
        {
            Username = signUpInfo.Username,
            Password = signUpInfo.Password,
            UserId = userId,
        };
    }
}
