using System;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class T1GetIndexHtmlTest : BaseTest
    {
        private const int Rps = 75;
        private const int Time = 30;
        
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
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(Rps, TimeSpan.FromSeconds(Time))
                );

            var result = RunScenario(scenario);

            VerifyResults(result, Rps * Time);
        }
    }
}