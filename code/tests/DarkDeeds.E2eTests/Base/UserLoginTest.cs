using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Base;

public class UserLoginTest : BaseTest
{
    protected override Task Test(Func<RemoteWebDriver, Task> action)
    {
        return base.Test(async driver =>
        {
            await CreateUserAndLogin(driver);
            await action(driver);
        });
    }
}
