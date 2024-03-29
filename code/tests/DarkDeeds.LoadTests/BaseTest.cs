using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Plugins.Network.Ping;
using Xunit;

namespace DarkDeeds.LoadTests;

[Collection("Performance Tests")]
[SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "It's implemented correctly. It's just not needed to call GC.SuppressFinalize(this) in this case.")]
public abstract class BaseTest : IDisposable
{
    protected static readonly Uri Url = new($"https://{Domain}");
    protected static readonly Config Config = new();

    private const string TestSuite = "LoadTests";
    private const string Password = "Qwerty!1";
    private static readonly string DateFolder = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    private readonly StringBuilder _output = new();

    protected static int Timeout => Config.Timeout;

    protected static int TimeWarmUp => Config.TimeWarmUp;

    protected static int TimeRamp => Config.TimeRamp;

    protected static int TimeTest => Config.TimeTest;

    protected abstract int RpsTest { get; }

    protected abstract int RpsMaxLatency { get; }

    protected int RpsWarmUp => Math.Max(1, (int)(0.1 * RpsTest));

    private static string Domain => Env.Domain;

    public void Dispose()
    {
        if (_output.Length > 0)
            File.AppendAllText(Path.Combine("reports", DateFolder, "output.txt"), _output.ToString());

        GC.SuppressFinalize(this);
    }

    protected string GetTestName()
    {
        return GetType().Name;
    }

    protected async Task<NodeStats> RunScenario(params Scenario[] scenarios)
    {
        var pingPluginConfig = PingPluginConfig.CreateDefault([Domain]);
        var pingPlugin = new PingPlugin(pingPluginConfig);

        var result = NBomberRunner
            .RegisterScenarios(scenarios)
            .WithTestSuite(TestSuite)
            .WithTestName(GetTestName())
            .WithReportFormats(ReportFormat.Txt, ReportFormat.Html)
            .WithReportFileName(GetTestName())
            .WithReportFolder(Path.Combine("reports", DateFolder, GetTestName()))
            .WithWorkerPlugins(pingPlugin)
            .Run();

        await Task.Delay(0);

        return result;
    }

    protected static string GenerateUsername()
    {
        return $"test-{Guid.NewGuid()}";
    }

    protected static async Task<string> CreateUserAndObtainToken(string username)
    {
        using var client = new HttpClient();
        client.BaseAddress = Url;
        var payload = JsonSerializer.Serialize(new { username, password = Password });
        using var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(new Uri("api/auth/account/signup", UriKind.Relative), content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseBodyParsed = JsonDocument.Parse(responseBody);
        var token = responseBodyParsed.RootElement.GetProperty("token").GetString();

        Assert.NotNull(token);

        return token;
    }

    protected void VerifyResults(NodeStats stats, Action? additionalChecks = null)
    {
        try
        {
            var totalCount = stats.ScenarioStats[0].OkCount + stats.ScenarioStats[0].FailCount;
            Assert.InRange(stats.ScenarioStats[0].OkCount, 0.99 * totalCount, totalCount);
            Assert.InRange(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent95, 0, RpsMaxLatency);
            additionalChecks?.Invoke();

            SaveResults(stats, true);
        }
        catch (Exception)
        {
            SaveResults(stats, false);
            throw;
        }
    }

    private void SaveResults(NodeStats stats, bool success)
    {
        var totalCount = stats.ScenarioStats[0].OkCount + stats.ScenarioStats[0].FailCount;
        var okPercent = Math.Round(100.0 * stats.ScenarioStats[0].OkCount / totalCount, 0);
        var rps = Math.Round(stats.ScenarioStats[0].StepStats[0].Ok.Request.RPS, 0);
        var p95 = Math.Round(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent95, 0);
        var p99 = Math.Round(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent99, 0);
        var name = Regex.Replace(GetTestName(), @"Test\d+", string.Empty);
        var tick = success ? "[X]" : "[ ]";
        _output.Append(CultureInfo.InvariantCulture, $"{tick} ");
        _output.Append(CultureInfo.InvariantCulture, $"{name.PadLeft(20)} ");
        _output.Append(CultureInfo.InvariantCulture, $"ok={okPercent.ToString(CultureInfo.InvariantCulture).PadRight(3)} ");
        _output.Append(CultureInfo.InvariantCulture, $"rps={rps.ToString(CultureInfo.InvariantCulture).PadRight(5)} ");
        _output.Append(CultureInfo.InvariantCulture, $"p95={p95.ToString(CultureInfo.InvariantCulture).PadRight(5)} ");
        _output.Append(CultureInfo.InvariantCulture, $"p99={p99.ToString(CultureInfo.InvariantCulture).PadRight(5)}");
        _output.AppendLine();
    }
}
