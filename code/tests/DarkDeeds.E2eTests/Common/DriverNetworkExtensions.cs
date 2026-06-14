using System.Text;
using System.Text.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace DarkDeeds.E2eTests.Common;

public static class DriverNetworkExtensions
{
    private static readonly Uri SeleniumGridUrl =
        new(Environment.GetEnvironmentVariable("SELENIUM_GRID_URL") ?? "http://localhost:4444");

    public static void GoOffline(this RemoteWebDriver driver)
    {
        driver.SetNetworkOffline(true);
    }

    public static void GoOnline(this RemoteWebDriver driver)
    {
        driver.SetNetworkOffline(false);
    }

    private static void SetNetworkOffline(this RemoteWebDriver driver, bool offline)
    {
        // ChromeDriver exposes network emulation through a typed API.
        if (driver is ChromeDriver chrome)
        {
            chrome.NetworkConditions = new ChromeNetworkConditions
            {
                IsOffline = offline,
                Latency = TimeSpan.Zero,
                DownloadThroughput = 0,
                UploadThroughput = 0,
            };
            return;
        }

        // RemoteWebDriver does not, so the chromium command is sent to the Grid over HTTP.
        SetNetworkOfflineOnGrid(driver, offline);
    }

    private static void SetNetworkOfflineOnGrid(RemoteWebDriver driver, bool offline)
    {
        var endpoint = new Uri(SeleniumGridUrl, $"wd/hub/session/{driver.SessionId}/chromium/network_conditions");
        var payload = JsonSerializer.Serialize(new
        {
            network_conditions = new
            {
                offline,
                latency = 0,
                download_throughput = 0,
                upload_throughput = 0,
            },
        });

        using var client = new HttpClient();
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = client.PostAsync(endpoint, content).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }
}
