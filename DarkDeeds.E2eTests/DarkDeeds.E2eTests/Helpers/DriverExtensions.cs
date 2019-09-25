using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

#pragma warning disable 618
// ExpectedConditions obsolete warning is suppressed as deprecated version works better than new one

namespace DarkDeeds.E2eTests.Helpers
{
    public static class DriverExtensions
    {
        private static WebDriverWait Wait(this RemoteWebDriver driver, int timeoutInSeconds = 15)
            => new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

        private static IWebElement GetElement(this RemoteWebDriver driver, string xpath)
        {
            driver.Wait().Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
            return driver.FindElementByXPath(xpath);
        }
        
        private static void WaitUntilDisappeared(this RemoteWebDriver driver, string xpath)
        {
            driver.Wait().Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(xpath)));
        }

        public static void SignIn(this RemoteWebDriver driver, string username, string password)
        {
            var inpUsername = driver.GetElement("//*[@data-id='inpUsername']/input");
            inpUsername.SendKeys(username);
            var inpPassword = driver.GetElement("//*[@data-id='inpPassword']/input");
            inpPassword.SendKeys(password);
            var buttonLogin = driver.GetElement("//*[@data-id='btnSignin']");
            buttonLogin.Click();
            driver.WaitUntilDisappeared("//*[@data-id='blockLogin']");
        }
    }
}

#pragma warning restore 618
