using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DarkDeeds.E2eTests.Common;
using DarkDeeds.E2eTests.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Base;

public class BaseTest
{
    private static readonly bool RunContainer = bool.Parse(Environment.GetEnvironmentVariable("RUN_CONTAINER") ?? "false");
    private static readonly string ArtifactsPath = Environment.GetEnvironmentVariable("ARTIFACTS_PATH") ?? "artifacts";
    protected static readonly string Url = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:3000";
    // TODO: remove this
    protected static readonly string ApiUrl = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:5000";
    private static readonly Random Random = new();

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

    protected virtual async Task Test(Func<RemoteWebDriver, Task> action)
    {
        using var driver = CreateDriver();
        try
        {
            await action(driver);
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

    protected async Task<TestUserDto> CreateUserAndLogin(RemoteWebDriver driver)
    {
        var testUser = await CreateUser();
        driver.SignIn(testUser.Username, testUser.Password);
        driver.WaitUntilUserLoaded();

        return testUser;
    }

    protected HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };

        return new HttpClient(handler)
        {
            BaseAddress = new Uri(ApiUrl)
        };
    }

    private async Task<TestUserDto> CreateUser()
    {
        using var client = CreateHttpClient();
        var result = await client.PostAsync("/api/test/CreateTestUser", null);
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TestUserDto>(content, JsonOptions.I);
    }
}
