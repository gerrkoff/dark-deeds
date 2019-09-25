using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests
{
    public class BaseTest
    {
        private static readonly string Url = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:3000";
        private static readonly bool RunContainer = bool.Parse(Environment.GetEnvironmentVariable("RUN_CONTAINER") ?? "false");
        protected static readonly string Username = Environment.GetEnvironmentVariable("USERNAME") ?? "qqq";
        protected static readonly string Password = Environment.GetEnvironmentVariable("PASSWORD") ?? "qqq";

        protected RemoteWebDriver CreateDriver()
        {
            Console.WriteLine(Url);
            ChromeOptions options = new ChromeOptions();
            if (RunContainer)
            {
                options.AddArguments("headless", "no-sandbox", "disable-gpu");
            }
            string driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var driver = new ChromeDriver(driverPath, options);
            driver.Navigate().GoToUrl(Url);
            return driver;
        }
    }
}