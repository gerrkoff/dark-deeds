using System;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test1GetIndexHtml : BaseTest
    {
        private const int Rps = 50;
        private const int Time = 30;
        private const int RampTime = 10;
        private const int WarmUpTime = 5;

        private readonly int _rpsMin = Math.Max(1, (int) (0.8 * Rps));
        private readonly int _rpsMax = Math.Max(1, (int) (1.2 * Rps));
        private readonly int _rpsWarmUp = Math.Max(1, (int) (0.1 * Rps));

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
                    Simulation.RampConstant(_rpsWarmUp, TimeSpan.FromSeconds(WarmUpTime)),
                    Simulation.RampPerSec(_rpsMin, TimeSpan.FromSeconds(RampTime)),
                    Simulation.InjectPerSecRandom(_rpsMin, _rpsMax, TimeSpan.FromSeconds(Time))
                );

            var result = RunScenario(scenario);

            VerifyResults(result);
        }
    }
}