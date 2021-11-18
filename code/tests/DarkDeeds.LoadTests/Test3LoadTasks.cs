using System;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test3LoadTasks : BaseTest
    {
        protected override int Rps => Env.Test3Rps;
        
        [Fact(Skip = "")]
        public async Task Test()
        {
            if (Rps == 0)
                return;
            
            var token = await CreateUserAndObtainToken(GenerateUsername());

            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", $"{Url}/web/api/tasks?from=2021-10-31T23:00:00.000Z")
                        .WithHeader("accept", "application/json")
                        .WithHeader("authorization", $"Bearer {token}");
            
                    return Http.Send(request, context);
                }, timeout: TimeSpan.FromSeconds(Timeout));

            var scenario = ScenarioBuilder
                .CreateScenario(GetTestName(), step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
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