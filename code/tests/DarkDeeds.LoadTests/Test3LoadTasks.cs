using System;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test3LoadTasks : BaseTest
    {
        protected override int RpsTest => Config.Test3Rps;

        [Fact]
        public async Task Test()
        {
            if (RpsTest == 0)
                return;

            var token = string.Empty;

            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", $"{Url}/api/task/tasks?from=2021-10-31T23:00:00.000Z")
                        .WithHeader("accept", "application/json")
                        .WithHeader("authorization", $"Bearer {token}");

                    return Http.Send(request, context);
                }, timeout: TimeSpan.FromSeconds(Timeout));

            var scenario = ScenarioBuilder
                .CreateScenario(GetTestName(), step)
                .WithInit(async context =>
                {
                    context.Logger.Information("Create user and obtain token");
                    token = await CreateUserAndObtainToken(GenerateUsername());
                    context.Logger.Information("Token obtained");
                })
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
