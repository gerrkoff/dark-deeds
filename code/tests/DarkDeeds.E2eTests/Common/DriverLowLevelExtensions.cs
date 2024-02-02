using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

#pragma warning disable 618
// ExpectedConditions obsolete warning is suppressed as deprecated version works better than new one

namespace DarkDeeds.E2eTests.Common;

public static class DriverLowLevelExtensions
{
    private static WebDriverWait Wait(this RemoteWebDriver driver, int timeoutInSeconds = 15) =>
        new(driver, TimeSpan.FromSeconds(timeoutInSeconds));

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

    public static bool ElementExists(this RemoteWebDriver driver, string xpath)
    {
        return driver.FindElementsByXPath(xpath).Count != 0;
    }

    public static void ScrollToElement(this RemoteWebDriver driver, IWebElement element)
    {
        driver.ExecuteJavaScript("arguments[0].scrollIntoView(true);", element);
        Thread.Sleep(50);
    }

    public static void TaskScreenshot(this RemoteWebDriver driver, string path, string name)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        driver.GetScreenshot().SaveAsFile(Path.Combine(path, $"{name}.png"));
    }

    public static void CreateTab(this RemoteWebDriver driver)
    {
        driver.ExecuteJavaScript("window.open()");
    }
}

#pragma warning restore 618
