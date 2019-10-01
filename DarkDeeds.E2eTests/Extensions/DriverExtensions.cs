using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Extensions
{
    public static class DriverExtensions
    {
        public static void SignIn(this RemoteWebDriver driver, string username, string password)
        {
            driver.GetUsernameInput().SendKeys(username);
            driver.GetPasswordInput().SendKeys(password);
            driver.GetSignInButton().Click();
            driver.WaitUntilDisappeared("//*[@data-id='loginComponent']");
        }

        public static void WaitUntillUserLoaded(this RemoteWebDriver driver)
        {
            driver.WaitUntilAppeared("//*[@data-id='overviewComponent']");
        }
        
        public static void WaitUntillSavingFinished(this RemoteWebDriver driver)
        {
            string xpath = "//*[@data-id='savingIndicator']";
            driver.WaitUntilAppeared(xpath);
            driver.WaitUntilDisappeared(xpath);
        }
    }
}
