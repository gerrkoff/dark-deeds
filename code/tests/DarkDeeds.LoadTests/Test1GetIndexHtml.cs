using System;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test1GetIndexHtml : BaseTest
    {
        protected override int Rps => Env.Test1Rps;

        [Fact]
        public async Task Test()
        {
            if (Rps == 0)
                return;
            
            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", Url)
                        .WithHeader("accept", "text/html");

                    return Http.Send(request, context);
                }, timeout: TimeSpan.FromSeconds(Timeout));

            var scenario = ScenarioBuilder
                .CreateScenario(GetTestName(), step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(WarmUpTime))
                .WithLoadSimulations(
                    Simulation.RampPerSec(RpsWarmUp, TimeSpan.FromSeconds(WarmUpTime)),
                    Simulation.RampPerSec(Rps, TimeSpan.FromSeconds(RampTime)),
                    Simulation.InjectPerSec(Rps, TimeSpan.FromSeconds(Time))
                    // Simulation.InjectPerSecRandom(RpsMin, RpsMax, TimeSpan.FromSeconds(Time))
                );

            var result = await RunScenario(scenario);

            VerifyResults(result);
        }
    }
}