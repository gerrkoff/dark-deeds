using System;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class GetIndexHtmlTest : BaseTest
    {
        private const int Rps = 10; // TODO: increase
        private const int Time = 30;
        
        [Fact]
        public void Test()
        {
            var step = Step.Create("get_index_html",
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", Url)
                        .WithHeader("Accept", "text/html");

                    return Http.Send(request, context);
                });

            var scenario = ScenarioBuilder
                .CreateScenario("Get Index Html", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(Rps, TimeSpan.FromSeconds(Time))
                );

            var result = RunScenario(scenario);

            Assert.Equal(Rps * Time, result.ScenarioStats[0].OkCount);
            Assert.Equal(0, result.ScenarioStats[0].FailCount);
            Assert.True(result.ScenarioStats[0].StepStats[0].Ok.Latency.Percent99 < 500);
        }
    }
}