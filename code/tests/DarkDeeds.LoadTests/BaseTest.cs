using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Plugins.Network.Ping;
using System.Text.Json;
using System.Text.RegularExpressions;
using NBomber.Configuration;
using Xunit;

namespace DarkDeeds.LoadTests
{
    [Collection("Performance Tests")]
    public abstract class BaseTest : IDisposable
    {
        private const string TestSuite = "LoadTests";
        private const string Password = "Qwerty!1";
        private static readonly string DateFolder = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        private static readonly string Domain = Env.Domain;
        private readonly StringBuilder _output = new();
        protected static readonly string Url = $"https://{Domain}";
        protected static readonly Config Config = new();

        protected int Timeout => Config.Timeout;
        protected int TimeWarmUp => Config.TimeWarmUp;
        protected int TimeRamp => Config.TimeRamp;
        protected int TimeTest => Config.TimeTest;
        protected abstract int RpsTest { get; }
        protected int RpsWarmUp => Math.Max(1, (int) (0.1 * RpsTest));

        protected string GetTestName() => GetType().Name;

        protected async Task<NodeStats> RunScenario(params Scenario[] scenarios)
        {
            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {Domain});
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

        protected string GenerateUsername() => $"test-{Guid.NewGuid()}";

        protected async Task<string> CreateUserAndObtainToken(string username)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(Url);
            var payload = JsonSerializer.Serialize(new {username, password = Password});
            var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("/api/auth/account/signup", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseBodyParsed = JsonDocument.Parse(responseBody);
            var token = responseBodyParsed.RootElement.GetProperty("token").GetString();

            return token;
        }

        protected async Task VerifyResults(NodeStats stats, Action additionalChecks = null)
        {
            try
            {
                var totalCount = stats.ScenarioStats[0].OkCount + stats.ScenarioStats[0].FailCount;
                Assert.InRange(stats.ScenarioStats[0].OkCount, 0.99 * totalCount, totalCount);
                Assert.InRange(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent95, 0, 500);
                additionalChecks?.Invoke();

                SaveResults(stats, true);
            }
            catch (Exception)
            {
                SaveResults(stats, false);
                throw;
            }
            finally
            {
                await Task.Delay(60_000);
            }
        }

        private void SaveResults(NodeStats stats, bool success)
        {
            var totalCount = stats.ScenarioStats[0].OkCount + stats.ScenarioStats[0].FailCount;
            var okPercent = Math.Round(100.0 * stats.ScenarioStats[0].OkCount / totalCount, 0);
            var rps = Math.Round(stats.ScenarioStats[0].StepStats[0].Ok.Request.RPS, 0);
            var p95 = Math.Round(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent95, 0);
            var p99 = Math.Round(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent99, 0);
            var name = Regex.Replace(GetTestName(), @"Test\d+", "");
            var tick = success ? "[X]" : "[ ]";
            _output.Append($"{tick} ");
            _output.Append($"{name.PadLeft(20)} ");
            _output.Append($"ok={okPercent.ToString(CultureInfo.InvariantCulture).PadRight(3)} ");
            _output.Append($"rps={rps.ToString(CultureInfo.InvariantCulture).PadRight(5)} ");
            _output.Append($"p95={p95.ToString(CultureInfo.InvariantCulture).PadRight(5)} ");
            _output.Append($"p99={p99.ToString(CultureInfo.InvariantCulture).PadRight(5)}");
            _output.AppendLine();
        }

        public void Dispose()
        {
            if (_output.Length > 0)
                File.AppendAllText(Path.Combine("reports", DateFolder, "output.txt"), _output.ToString());
        }
    }
}
