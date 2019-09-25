using System.IO;
using System.Reflection;
using DarkDeeds.E2eTests.Helpers;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests
{
    public class BaseTest
    {
        private static readonly string Url;
        private static readonly bool RunInContainer;
        protected static readonly string Username;
        protected static readonly string Password;
        
        static BaseTest()
        {
            string settingsSerialized = File.ReadAllText("settings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(settingsSerialized);
            Url = settings.Url;
            Username = settings.Username;
            Password = settings.Password;
            RunInContainer = settings.RunInContainer;
        }
        
        protected RemoteWebDriver CreateDriver()
        {
            ChromeOptions options = new ChromeOptions();
            if (RunInContainer)
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