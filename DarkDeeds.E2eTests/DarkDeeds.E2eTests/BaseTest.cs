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
        protected static readonly string Url;
        protected static readonly string Username;
        protected static readonly string Password;
        protected static readonly bool RunInContainer;
        
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
                options.AddArguments("headless");
                options.AddArguments("no-sandbox");    
            }
            var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
            driver.Navigate().GoToUrl(Url);
            return driver;
        }
    }
}