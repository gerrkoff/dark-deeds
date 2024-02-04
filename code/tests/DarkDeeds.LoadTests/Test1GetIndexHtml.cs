using System;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test1GetIndexHtml : BaseTest
    {
        protected override int RpsTest => Config.Test1Rps;

        [Fact]
        public async Task Test()
        {
            if (RpsTest == 0)
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
                .WithWarmUpDuration(TimeSpan.FromSeconds(TimeWarmUp))
                .WithLoadSimulations(
                    Simulation.RampPerSec(RpsWarmUp, TimeSpan.FromSeconds(TimeWarmUp)),
                    Simulation.RampPerSec(RpsTest, TimeSpan.FromSeconds(TimeRamp)),
                    Simulation.InjectPerSec(RpsTest, TimeSpan.FromSeconds(TimeTest))
                    // Simulation.InjectPerSecRandom(RpsMin, RpsMax, TimeSpan.FromSeconds(Time))
                )
                .WithClean(async context =>
                {
                    context.Logger.Information("Cool down");
                    await Task.Delay(Config.Cooldown * 1000);
                    context.Logger.Information("Move on");
                });

            var result = await RunScenario(scenario);

            VerifyResults(result);
        }
    }
}
