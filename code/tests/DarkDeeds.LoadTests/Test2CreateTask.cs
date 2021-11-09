using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test2CreateTask : BaseTest
    {
        protected override int Rps => Env.Test2Rps;
        
        [Fact(Skip = "")]
        public async Task Test()
        {
            var token = await CreateUserAndObtainToken(GenerateUsername());

            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("POST", $"{Url}/web/api/tasks")
                        .WithHeader("accept", "application/json")
                        .WithHeader("authorization", $"Bearer {token}")
                        .WithBody(JsonContent.Create(new object[]
                        {
                            new
                            {
                                title = "create_task_test",
                                uid = Guid.NewGuid(),
                            }
                        }));
            
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
            
            var result = RunScenario(scenario);
            
            VerifyResults(result);
        }
    }
}