using System.Net.Http.Json;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests;

public class Test4CreateTaskSequential : BaseTest
{
    protected override int RpsTest => Config.Test4Rps;

    protected override int RpsMaxLatency => Config.Test4MaxLatency;

    [Fact]
    public async Task Test()
    {
        if (RpsTest == 0)
            return;

        var token = string.Empty;

        var step = Step.Create(
            GetTestName(),
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
                        },
                    }));

                return Http.Send(request, context);
            },
            timeout: TimeSpan.FromSeconds(Timeout));

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
                Simulation.KeepConstant(RpsTest, TimeSpan.FromSeconds(TimeTest)));

        var result = await RunScenario(scenario);

        VerifyResults(result, () =>
        {
            Assert.NotInRange(result.ScenarioStats[0].StepStats[0].Ok.Request.RPS, 0, 5 * RpsTest);
        });
    }
}
