using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests
{
    public class Test4CreateTaskSequential : BaseTest
    {
        protected override int RpsTest => Config.Test4Rps;

        [Fact]
        public async Task Test()
        {
            if (RpsTest == 0)
                return;

            var token = await CreateUserAndObtainToken(GenerateUsername());

            var step = Step.Create(GetTestName(),
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("POST", $"{Url}/api/task/tasks")
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
                    Simulation.KeepConstant(RpsTest, TimeSpan.FromSeconds(TimeTest))
                );

            var result = await RunScenario(scenario);

            await VerifyResults(result, () =>
            {
                Assert.NotInRange(result.ScenarioStats[0].StepStats[0].Ok.Request.RPS, 0, 5 * RpsTest);
            });
        }
    }
}
