using System;
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
using NBomber.Configuration;
using Xunit;

namespace DarkDeeds.LoadTests
{
    [Collection("")]
    public abstract class BaseTest
    {
        private const string TestSuite = "LoadTests";
        private const string Password = "Qwerty!1";
        private static readonly string DateFolder = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        private static readonly string Domain = Env.Domain;
        protected static readonly string Url = $"https://{Domain}";
        protected static readonly Config Config = new();
        
        protected int Timeout => Config.Timeout;
        protected int TimeWarmUp => Config.TimeWarmUp;
        protected int TimeRamp => Config.TimeRamp;
        protected int TimeTest => Config.TimeTest;
        protected abstract int RpsTest { get; }
        protected int RpsWarmUp => Math.Max(1, (int) (0.1 * RpsTest));

        protected BaseTest()
        {
            Assert.NotEmpty(Domain);
        }

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

            await Task.Delay(10000);

            return result;
        }

        protected string GenerateUsername() => $"test-{Guid.NewGuid()}";
        
        protected async Task<string> CreateUserAndObtainToken(string username)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri(Url)
            };
            var payload = JsonSerializer.Serialize(new {username, password = Password});
            var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("/web/api/account/signup", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseBodyParsed = JsonDocument.Parse(responseBody);
            var token = responseBodyParsed.RootElement.GetProperty("token").GetString();

            return token;
        }

        protected void VerifyResults(NodeStats stats)
        {
            var totalCount = stats.ScenarioStats[0].OkCount + stats.ScenarioStats[0].FailCount;
            Assert.InRange(stats.ScenarioStats[0].OkCount, 0.99 * totalCount, totalCount);
            Assert.InRange(stats.ScenarioStats[0].StepStats[0].Ok.Latency.Percent95, 0, 500);
        }
    }
}