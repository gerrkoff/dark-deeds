using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

#pragma warning disable 618
// ExpectedConditions obsolete warning is suppressed as deprecated version works better than new one

namespace DarkDeeds.E2eTests.Extensions
{
    public static class DriverLowLevelExtensions
    {   
        private static WebDriverWait Wait(this RemoteWebDriver driver, int timeoutInSeconds = 15)
            => new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        
        public static void WaitUntilAppeared(this RemoteWebDriver driver, string xpath)
        {
            driver.Wait().Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
        }
        
        public static void WaitUntilDisappeared(this RemoteWebDriver driver, string xpath)
        {
            driver.Wait().Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(xpath)));
        }
        
        public static IWebElement GetElement(this RemoteWebDriver driver, string xpath)
        {
            driver.Wait().Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
            return driver.FindElementByXPath(xpath);
        }
    }
}

#pragma warning restore 618
