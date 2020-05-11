using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using DarkDeeds.E2eTests.Common;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests
{
    public class BaseTest
    {
        private static readonly bool RunContainer = bool.Parse(Environment.GetEnvironmentVariable("RUN_CONTAINER") ?? "false");
        private static readonly string ArtifactsPath = Environment.GetEnvironmentVariable("ARTIFACTS_PATH") ?? "artifacts";
        protected static readonly string Url = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:3000";
        protected static readonly string ApiUrl = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:5000";
        protected static readonly string Username = Environment.GetEnvironmentVariable("USERNAME") ?? "qqq";
        protected static readonly string Password = Environment.GetEnvironmentVariable("PASSWORD") ?? "qqq";
        
        private static readonly Random Random = new Random();

        private RemoteWebDriver CreateDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--ignore-certificate-errors");
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
                    driver.TaskScreenshot(ArtifactsPath, screenshotName);
                    throw;
                }
            }
        }

        protected string RandomizeText(string text)
        {
            return $"{text} {Random.Next()}";
        }

        protected HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };

            return new HttpClient(handler);
        }
    }
}