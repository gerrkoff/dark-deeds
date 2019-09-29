using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit;

namespace DarkDeeds.E2eTests
{
    public class BaseTest
    {
        private static readonly string Url = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:3000";
        private static readonly bool RunContainer = bool.Parse(Environment.GetEnvironmentVariable("RUN_CONTAINER") ?? "false");
        private static readonly string ArtifactsPath = Environment.GetEnvironmentVariable("ARTIFACTS_PATH") ?? string.Empty;
        protected static readonly string Username = Environment.GetEnvironmentVariable("USERNAME") ?? "qqq";
        protected static readonly string Password = Environment.GetEnvironmentVariable("PASSWORD") ?? "qqq";
        
        private static Random _random = new Random();

        private RemoteWebDriver CreateDriver()
        {
            ChromeOptions options = new ChromeOptions();
            if (RunContainer)
            {
                options.AddArguments("headless", "no-sandbox");
            }
            string driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var driver = new ChromeDriver(driverPath, options);
            driver.Navigate().GoToUrl(Url);
            return driver;
        }

        protected void Test(Action<RemoteWebDriver> action)
        {
            using (var driver = CreateDriver())
            {
                try
                {
                    action(driver);
                }
                catch (Exception)
                {
                    var testName = new StackTrace().GetFrame(1).GetMethod().Name;
                    var screenshotName = $"{GetType().Name}__{testName}";
                    driver.GetScreenshot().SaveAsFile(Path.Combine(ArtifactsPath, $"{screenshotName}.png"));
                    throw;
                }
            }
        }

        protected string RandomizeText(string text)
        {
            return $"{text} {_random.Next()}";
        }
    }
}