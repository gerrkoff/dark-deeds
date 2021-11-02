using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class CreateTaskTest : BaseTest
    {
        private const int Rps = 1; // TODO: increase
        private const int Time = 10;
        
        [Fact]
        public async Task Test()
        {
            var token = await CreateUserAndObtainToken(GenerateUsername());

            var step = Step.Create("create_task",
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
                });

            var scenario = ScenarioBuilder
                .CreateScenario("Create Task", step)
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