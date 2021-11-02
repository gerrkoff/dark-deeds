using System;
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
        private static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? "";
        protected static readonly string Url = $"https://{Domain}";

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
    }
}