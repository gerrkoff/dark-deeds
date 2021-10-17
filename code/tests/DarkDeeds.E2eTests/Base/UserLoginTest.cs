using System;
using System.Threading.Tasks;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Base
{
    public class UserLoginTest : BaseTest
    {        
        protected override Task Test(Func<RemoteWebDriver, Task> action) => base.Test(async driver =>
        {
            await CreateUserAndLogin(driver, Guid.NewGuid().ToString());
            await action(driver);
        });
    }
}