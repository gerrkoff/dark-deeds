using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using DarkDeeds.E2eTests.Common;
using DarkDeeds.E2eTests.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit;

namespace DarkDeeds.E2eTests.Base;

[Collection("Sequential")]
public class BaseTest
{
    protected static readonly Uri Url = new(Environment.GetEnvironmentVariable("URL") ?? "http://localhost:3000");
    private static readonly Uri BackendUrl = new(Environment.GetEnvironmentVariable("BE_URL") ?? "http://localhost:5000");
    private static readonly Uri SeleniumGridUrl = new(Environment.GetEnvironmentVariable("SELENIUM_GRID_URL") ?? "http://localhost:4444");
    private static readonly string ArtifactsPath = Environment.GetEnvironmentVariable("ARTIFACTS_PATH") ?? "artifacts";
    private static readonly bool IsContainer = Environment.GetEnvironmentVariable("CONTAINER") == "true";
    private static readonly Random Random = new();

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

    protected async Task Test(Action<RemoteWebDriver> action)
    {
        await Test(driver =>
        {
            action(driver);
            return Task.CompletedTask;
        });
    }

    protected static string RandomizeText(string text)
    {
        return $"{text} {Random.Next()}";
    }

    protected static async Task<TestUserDto> CreateUserAndLogin(RemoteWebDriver driver)
    {
        var testUser = await CreateUser();
        driver.SignIn(testUser.Username, testUser.Password);
        driver.WaitUntilUserLoaded();

        return testUser;
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpClientHandler is disposed at the end of the tests")]
    protected static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };

        return new HttpClient(handler)
        {
            BaseAddress = BackendUrl,
        };
    }

    private static RemoteWebDriver CreateDriver()
    {
        var options = new ChromeOptions();
        options.AddArguments("--ignore-certificate-errors");
        options.AddArguments("--verbose");
        if (IsContainer)
        {
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--disable-gpu");
            options.AddArguments("--remote-debugging-port=9222");
            options.AddArguments("--headless");
        }

        var driver = IsContainer
            ? new RemoteWebDriver(new Uri($"{SeleniumGridUrl}wd/hub"), options)
            : new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);

        driver.Navigate().GoToUrl(Url);
        return driver;
    }

    private static async Task<TestUserDto> CreateUser()
    {
        using var client = CreateHttpClient();
        var result = await client.PostAsync(new Uri("api/test/CreateTestUser", UriKind.Relative), null);
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<TestUserDto>(content, JsonOptions.I);

        ArgumentNullException.ThrowIfNull(user);
        return user;
    }
}
