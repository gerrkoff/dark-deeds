using System;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test1GetIndexHtml : BaseTest
    {
        protected override int Rps => 50;

        [Fact]
        public void Test()
        {   
            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", Url)
                        .WithHeader("accept", "text/html");

                    return Http.Send(request, context);
                });

            var scenario = ScenarioBuilder
                .CreateScenario(GetTestName(), step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(WarmUpTime))
                .WithLoadSimulations(
                    Simulation.RampConstant(RpsWarmUp, TimeSpan.FromSeconds(WarmUpTime)),
                    Simulation.RampPerSec(RpsMin, TimeSpan.FromSeconds(RampTime)),
                    Simulation.InjectPerSecRandom(RpsMin, RpsMax, TimeSpan.FromSeconds(Time))
                );

            var result = RunScenario(scenario);

            VerifyResults(result);
        }
    }
}