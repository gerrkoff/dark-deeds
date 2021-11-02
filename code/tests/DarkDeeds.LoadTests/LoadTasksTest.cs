using System;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class LoadTasksTest : BaseTest
    {
        private const int Rps = 10;
        private const int Time = 30;
        
        [Fact]
        public async Task Test()
        {
            var token = await CreateUserAndObtainToken(GenerateUsername());

            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", $"{Url}/web/api/tasks?from=2021-10-31T23:00:00.000Z")
                        .WithHeader("accept", "application/json")
                        .WithHeader("authorization", $"Bearer {token}");
            
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