using System;
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
using Xunit;

namespace DarkDeeds.LoadTests
{
    [Collection("")]
    public abstract class BaseTest
    {
        private const string TestSuite = "LoadTests";
        private static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? "test.dark-deeds.com";
        protected static readonly string Url = $"https://{Domain}";
        private const string Password = "Qwerty!1";

        protected BaseTest()
        {
            Assert.NotEmpty(Domain);
        }

        private string GetTestName() => GetType().Name;

        protected NodeStats RunScenario(params Scenario[] scenarios)
        {
            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {Domain});
            var pingPlugin = new PingPlugin(pingPluginConfig);

            return NBomberRunner
                .RegisterScenarios(scenarios)
                .WithTestSuite(TestSuite)
                .WithTestName(GetTestName())
                .WithWorkerPlugins(pingPlugin)
                .Run();
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
    }
}