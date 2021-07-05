using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DarkDeeds.E2eTests.Common;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests
{
    public class BaseTest
    {
        private const string Password = "Qwerty6^";
        private static readonly string Username = Guid.NewGuid().ToString();
        
        private static readonly bool RunContainer = bool.Parse(Environment.GetEnvironmentVariable("RUN_CONTAINER") ?? "false");
        private static readonly string ArtifactsPath = Environment.GetEnvironmentVariable("ARTIFACTS_PATH") ?? "artifacts";
        protected static readonly string Url = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:5000";
        
        private static readonly Random Random = new Random();

        private RemoteWebDriver CreateDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--ignore-certificate-errors");
            if (RunContainer)
            {
                options.AddArguments("headless", "no-sandbox", "--verbose");
            }
            string driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var driver = new ChromeDriver(driverPath, options);
            driver.Navigate().GoToUrl(Url);
            return driver;
        }

        protected async Task Test(Action<RemoteWebDriver> action)
        {
            using var driver = CreateDriver();
            try
            {
                await CreateUser();
                driver.SignIn(Username, Password);
                driver.WaitUntilUserLoaded();

                action(driver);
            }
            catch (Exception)
            {
                var testName = new StackTrace().GetFrame(1)?.GetMethod()?.Name;
                var screenshotName = $"{GetType().Name}__{testName}";
                driver.TaskScreenshot(ArtifactsPath, screenshotName);
                throw;
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
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            return new HttpClient(handler);
        }

        private async Task CreateUser()
        {
            using var client = CreateHttpClient();
            var payload = JsonConvert.SerializeObject(new {username = Username, password = Password});
            var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync($"{Url}/web/api/account/signup", content);
        }
    }
}