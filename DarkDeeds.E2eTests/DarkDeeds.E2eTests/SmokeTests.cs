using DarkDeeds.E2eTests.Helpers;
using Xunit;

namespace DarkDeeds.E2eTests
{
    public class SmokeTests : BaseTest
    {
        [Fact]
        public void SignInTest()
        {
            using (var driver = Common.CreateDriver(Url))
            {
                driver.SignIn(Username, Password);
            }
        }
    }
}