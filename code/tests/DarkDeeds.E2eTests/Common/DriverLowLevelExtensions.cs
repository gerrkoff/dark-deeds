using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

#pragma warning disable 618
// ExpectedConditions obsolete warning is suppressed as deprecated version works better than new one
namespace DarkDeeds.E2eTests.Common;

public static class DriverLowLevelExtensions
{
    public static void WaitUntilAppeared(this RemoteWebDriver driver, string xpath)
    {
        driver.Wait().Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
    }

    public static void WaitUntilDisappeared(this RemoteWebDriver driver, string xpath)
    {
        driver.Wait().Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(xpath)));
    }

    public static void WaitUntilServiceWorkerControls(this RemoteWebDriver driver)
    {
        driver.Wait().Until(_ => driver.ExecuteJavaScript<bool>(
            "return 'serviceWorker' in navigator && navigator.serviceWorker.controller != null;"));
    }

    // A busy indicator (e.g. the "saving"/"loading" spinner) can appear and disappear within a
    // few milliseconds when the backend responds fast (e.g. over a local network). The fixed
    // polling interval of WebDriverWait then misses the flash entirely, so a plain "wait until
    // appeared" times out even though the operation succeeded. This waits until the indicator is
    // reliably gone: it returns as soon as the element has appeared and then disappeared, or once
    // it has stayed absent long enough to be sure the (synchronously triggered) operation has
    // already finished. It still fails if the indicator stays present until the timeout.
    public static void WaitUntilTransientCompleted(this RemoteWebDriver driver, string xpath)
    {
        var timeout = TimeSpan.FromSeconds(15);
        var settle = TimeSpan.FromMilliseconds(150);
        var poll = TimeSpan.FromMilliseconds(20);

        var deadline = DateTime.UtcNow + timeout;
        var everSeen = false;
        DateTime? absentSince = null;

        while (DateTime.UtcNow < deadline)
        {
            if (driver.ElementExists(xpath))
            {
                everSeen = true;
                absentSince = null;
            }
            else
            {
                if (everSeen)
                    return;

                absentSince ??= DateTime.UtcNow;
                if (DateTime.UtcNow - absentSince >= settle)
                    return;
            }

            Thread.Sleep(poll);
        }

        throw new WebDriverTimeoutException($"Transient element '{xpath}' was still present after {timeout.TotalSeconds:0}s");
    }

    public static IWebElement GetElement(this RemoteWebDriver driver, string xpath)
    {
        driver.Wait().Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
        return driver.FindElementByXPath(xpath);
    }

    // A freshly opened modal animates in, so its input can already exist in the DOM while still
    // being non-interactable for a few frames. Typing into it then throws ElementNotInteractable.
    // This waits until the element is visible and enabled before returning it.
    public static IWebElement GetInteractableElement(this RemoteWebDriver driver, string xpath)
    {
        return driver.Wait().Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
    }

    public static int CountElements(this RemoteWebDriver driver, string xpath)
    {
        return driver.FindElements(By.XPath(xpath)).Count;
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
            Directory.CreateDirectory(path);

        driver.GetScreenshot().SaveAsFile(Path.Combine(path, $"{name}.png"));
    }

    public static void CreateTab(this RemoteWebDriver driver)
    {
        driver.ExecuteJavaScript("window.open()");
    }

    public static DateTime GetBrowserDate(this RemoteWebDriver driver)
    {
        var date = driver.ExecuteJavaScript<string>(
            "const d = new Date();" +
            "return d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0');");
        return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static WebDriverWait Wait(this RemoteWebDriver driver, int timeoutInSeconds = 15)
    {
        return new(driver, TimeSpan.FromSeconds(timeoutInSeconds));
    }
}

#pragma warning restore 618
