using Xunit;

namespace DarkDeeds.E2eTests
{
    public class SmokeTests : BaseTest
    {
        [Fact]
        public void SignInTest()
        {
            Test(driver =>
            {
                driver.SignIn(Username, Password);
            });
        }
    }
}