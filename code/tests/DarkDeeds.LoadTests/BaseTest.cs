using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Plugins.Network.Ping;
using Xunit;

namespace DarkDeeds.LoadTests
{
    [Collection("")]
    public abstract class BaseTest
    {
        private const string TestSuite = "LoadTests";
        private const string Domain = "dark-deeds.com";
        protected readonly string Url = $"https://{Domain}";

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
    }
}