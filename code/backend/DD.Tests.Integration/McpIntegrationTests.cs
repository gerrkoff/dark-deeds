using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using ModelContextProtocol.Protocol;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration;

public sealed class McpIntegrationTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private static readonly TimeSpan ProtocolTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Mcp_RejectsLoginRefreshAndAuthorizationCodeTokens_AndScopesOAuthAccessToken()
    {
        await using var session = await OAuthMcpTestSession.CreateAsync();

        await AssertInitializationRejectedAsync(session.LoginToken);
        await AssertInitializationRejectedAsync(session.RefreshToken!);
        await AssertInitializationRejectedAsync(session.AuthorizationCode);

        await using var mcpClient = await McpTestClient.ConnectAsync(
            session.AccessToken!,
            CreateTimeoutToken());
        using var restClient = await CreateClientAsync();
        restClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", session.AccessToken);

        using var response = await restClient.GetAsync(
            CreateTasksUri(DateTime.UtcNow.Date),
            CreateTimeoutToken());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Mcp_ListsOnlySupportedTools()
    {
        await using var session = await OAuthMcpTestSession.CreateAsync();
        await using var mcpClient = await McpTestClient.ConnectAsync(
            session.AccessToken!,
            CreateTimeoutToken());

        var tools = await mcpClient.Client.ListToolsAsync(
            cancellationToken: CreateTimeoutToken());

        Assert.Equal(
            ["AddTasks", "LoadTasks", "UpdateTasksOrder"],
            tools.Select(tool => tool.Name).Order(StringComparer.Ordinal));
    }

    [Fact]
    public async Task Mcp_AddAndLoadTasks_ReturnsJsonTextAndHonorsExclusiveDateRange()
    {
        await using var session = await OAuthMcpTestSession.CreateAsync();
        await using var mcpClient = await McpTestClient.ConnectAsync(
            session.AccessToken!,
            CreateTimeoutToken());

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var todayTitle = $"MCP today {Guid.NewGuid():N}";
        var tomorrowTitle = $"MCP tomorrow {Guid.NewGuid():N}";

        var addResult = await mcpClient.Client.CallToolAsync(
            "AddTasks",
            new Dictionary<string, object?>
            {
                ["tasks"] =
                    new[]
                    {
                        new TaskCreateDto
                        {
                            Title = todayTitle,
                            Date = today,
                            Time = 615,
                            Type = TaskTypeDto.Routine,
                            IsProbable = true,
                        },
                        new TaskCreateDto
                        {
                            Title = tomorrowTitle,
                            Date = tomorrow,
                            Type = TaskTypeDto.Additional,
                        },
                    },
                ["justification"] = "Integration coverage for MCP task creation.",
            },
            cancellationToken: CreateTimeoutToken());

        var addText = ReadTaskText(addResult);
        using var addJson = JsonDocument.Parse(addText);
        var routineTaskJson = Assert.Single(
            addJson.RootElement.EnumerateArray(),
            task => task.GetProperty(nameof(TaskDto.Title)).GetString() == todayTitle);
        Assert.Equal(
            nameof(TaskTypeDto.Routine),
            routineTaskJson.GetProperty(nameof(TaskDto.Type)).GetString());

        var addedTasks = JsonSerializer.Deserialize<TaskDto[]>(addText, JsonOptions)
            ?? throw new InvalidOperationException("MCP tool returned empty task JSON.");
        Assert.Equal(2, addedTasks.Length);
        Assert.Contains(
            addedTasks,
            task => task.Title == todayTitle &&
                task.Date == today &&
                task.Time == 615 &&
                task.Type == TaskTypeDto.Routine &&
                task.IsProbable);
        Assert.Contains(
            addedTasks,
            task => task.Title == tomorrowTitle &&
                task.Date == tomorrow &&
                task.Type == TaskTypeDto.Additional);

        var loadResult = await mcpClient.Client.CallToolAsync(
            "LoadTasks",
            new Dictionary<string, object?>
            {
                ["from"] = today,
                ["till"] = tomorrow,
            },
            cancellationToken: CreateTimeoutToken());

        var loadedTasks = ReadTaskResult(loadResult);
        var loadedTodayTask = Assert.Single(loadedTasks, task => task.Title == todayTitle);
        Assert.Equal(today, loadedTodayTask.Date);
        Assert.DoesNotContain(loadedTasks, task => task.Title == tomorrowTitle);

        using var restResponse = await session.LoginHttpClient.GetAsync(
            CreateTasksUri(today),
            CreateTimeoutToken());
        var persistedTasks = await ReadTasksAsync(restResponse);
        Assert.Contains(persistedTasks, task => task.Title == todayTitle);
        Assert.Contains(persistedTasks, task => task.Title == tomorrowTitle);
    }

    [Fact]
    public async Task Mcp_UpdateTasksOrder_UpdatesOnlyEligibleTasksAndNotifiesWithoutSuppression()
    {
        await using var session = await OAuthMcpTestSession.CreateAsync();
        await using var foreignUser = await CreateUserClientAsync();
        var today = DateTime.UtcNow.Date;

        var validTask = CreateTask("MCP valid task", today, 1);
        var secondValidTask = CreateTask("MCP second valid task", today, 2);
        var deletedTask = CreateTask("MCP deleted task", today, 3);
        var foreignTask = CreateTask("MCP foreign task", today, 5);

        _ = await SaveTasksAsync(session.LoginHttpClient, validTask, secondValidTask, deletedTask);
        _ = await SaveTasksAsync(foreignUser.HttpClient, foreignTask);

        deletedTask.Deleted = true;
        var deletedResponse = await SaveTasksAsync(session.LoginHttpClient, deletedTask);
        Assert.True(Assert.Single(deletedResponse).Deleted);

        using var signalRHandler = await CreateSignalRHandlerAsync();
        await using var collector = await SignalRUpdateCollector.ConnectAsync(
            signalRHandler,
            session.LoginToken);
        await using var mcpClient = await McpTestClient.ConnectAsync(
            session.AccessToken!,
            CreateTimeoutToken());

        var updateResult = await mcpClient.Client.CallToolAsync(
            "UpdateTasksOrder",
            new Dictionary<string, object?>
            {
                ["updates"] =
                    new[]
                    {
                        new TaskUpdateDto { Uid = validTask.Uid, Order = 101 },
                        new TaskUpdateDto { Uid = Guid.NewGuid().ToString(), Order = 102 },
                        new TaskUpdateDto { Uid = deletedTask.Uid, Order = 103 },
                        new TaskUpdateDto { Uid = foreignTask.Uid, Order = 104 },
                        new TaskUpdateDto { Uid = secondValidTask.Uid, Order = 105 },
                    },
                ["justification"] = "Integration coverage for MCP task ordering.",
            },
            cancellationToken: CreateTimeoutToken());

        var updatedTasks = ReadTaskResult(updateResult);
        Assert.Equal(2, updatedTasks.Length);
        Assert.Contains(
            updatedTasks,
            task => task.Uid == validTask.Uid && task.Order == 101 && task.Version == 2);
        Assert.Contains(
            updatedTasks,
            task => task.Uid == secondValidTask.Uid && task.Order == 105 && task.Version == 2);

        await collector.WaitForTaskAsync(
            task => task.Uid == validTask.Uid && task.Order == 101 && task.Version == 2,
            ProtocolTimeout);
        await collector.WaitForTaskAsync(
            task => task.Uid == secondValidTask.Uid && task.Order == 105 && task.Version == 2,
            ProtocolTimeout);

        using var restResponse = await session.LoginHttpClient.GetAsync(
            CreateTasksUri(today),
            CreateTimeoutToken());
        var persistedTasks = await ReadTasksAsync(restResponse);
        AssertTaskOrder(persistedTasks, validTask.Uid, 101, 2);
        AssertTaskOrder(persistedTasks, secondValidTask.Uid, 105, 2);
        AssertTaskOrder(persistedTasks, deletedTask.Uid, 3, 2);
        Assert.DoesNotContain(persistedTasks, task => task.Uid == foreignTask.Uid);

        using var foreignRestResponse = await foreignUser.HttpClient.GetAsync(
            CreateTasksUri(today),
            CreateTimeoutToken());
        var foreignPersistedTasks = await ReadTasksAsync(foreignRestResponse);
        AssertTaskOrder(foreignPersistedTasks, foreignTask.Uid, 5, 1);
    }

    private static async Task AssertInitializationRejectedAsync(string token)
    {
        using var cancellationTokenSource = new CancellationTokenSource(ProtocolTimeout);
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await using var client = await McpTestClient.ConnectAsync(
                token,
                cancellationTokenSource.Token);
        });
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    private static async Task<TaskDto[]> SaveTasksAsync(HttpClient client, params TaskDto[] tasks)
    {
        using var response = await client.PostAsJsonAsync(
            "api/task/tasks",
            tasks,
            cancellationToken: CreateTimeoutToken());
        return await ReadTasksAsync(response);
    }

    private static TaskDto CreateTask(string title, DateTime date, int order)
    {
        return new TaskDto
        {
            Uid = CreateUniqueTaskUid(),
            Title = title,
            Date = date,
            Order = order,
            Type = TaskTypeDto.Simple,
        };
    }

    private static void AssertTaskOrder(
        IEnumerable<TaskDto> tasks,
        string uid,
        int order,
        int version)
    {
        var task = Assert.Single(tasks, item => item.Uid == uid);
        Assert.Equal(order, task.Order);
        Assert.Equal(version, task.Version);
    }

    private static TaskDto[] ReadTaskResult(CallToolResult result)
    {
        var text = ReadTaskText(result);
        return JsonSerializer.Deserialize<TaskDto[]>(text, JsonOptions)
               ?? throw new InvalidOperationException("MCP tool returned empty task JSON.");
    }

    private static string ReadTaskText(CallToolResult result)
    {
        Assert.NotEqual(true, result.IsError);
        var textBlock = Assert.IsType<TextContentBlock>(Assert.Single(result.Content));
        Assert.NotEmpty(textBlock.Text);
        return textBlock.Text;
    }

    private static CancellationToken CreateTimeoutToken()
    {
        return new CancellationTokenSource(ProtocolTimeout).Token;
    }
}
