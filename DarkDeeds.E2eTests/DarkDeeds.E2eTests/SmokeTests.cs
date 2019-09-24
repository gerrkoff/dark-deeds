using DarkDeeds.E2eTests.Helpers;
using Xunit;

namespace DarkDeeds.E2eTests
{
    public class SmokeTests : BaseTest
    {
        [Fact]
        public void SignInTest()
        {
            using (var driver = CreateDriver())
            {
                driver.SignIn(Username, Password);
            }
        }
    }
}