using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

#pragma warning disable 618
// ExpectedConditions obsolete warning is suppressed as deprecated version works better than new one

namespace DarkDeeds.E2eTests.Helpers
{
    public static class Common
    {
        public static RemoteWebDriver CreateDriver(string url)
        {
            var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            driver.Navigate().GoToUrl(url);
            return driver;
        } 

        private static WebDriverWait Wait(this RemoteWebDriver driver, int timeoutInSeconds = 15)
            => new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

        public static void SignIn(this RemoteWebDriver driver, string username, string password)
        {
            var inpUsername = driver.FindElementByXPath("//*[@data-id='inpUsername']/input");
            inpUsername.SendKeys(username);
            var inpPassword = driver.FindElementByXPath("//*[@data-id='inpPassword']/input");
            inpPassword.SendKeys(password);
            var buttonLogin = driver.FindElementByXPath("//*[@data-id='btnSignin']");
            buttonLogin.Click();
            driver.Wait().Until(ExpectedConditions.InvisibilityOfElementLocated(
                By.XPath("//*[@data-id='blockLogin']")));
        }
    }
}

#pragma warning restore 618