using System.Net.Http.Json;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using Xunit;

namespace DarkDeeds.LoadTests;

public class Test2CreateTask : BaseTest
{
    protected override int RpsTest => Config.Test2Rps;

    protected override int RpsMaxLatency => Config.Test2MaxLatency;

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
                Simulation.RampPerSec(RpsWarmUp, TimeSpan.FromSeconds(TimeWarmUp)),
                Simulation.RampPerSec(RpsTest, TimeSpan.FromSeconds(TimeRamp)),
                Simulation.InjectPerSec(RpsTest, TimeSpan.FromSeconds(TimeTest)));

        var result = await RunScenario(scenario);

        VerifyResults(result);
    }
}
