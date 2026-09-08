using System.Net;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Infrastructure;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration;

public sealed class TasksIsolationIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task Tasks_FilterActual_ReturnsCurrentFutureAndOverdueIncompleteSimpleTasks()
    {
        await using var user = await CreateUserClientAsync();
        var from = DateTime.UtcNow.Date;
        var currentTask = CreateTask("Current task", from);
        var futureTask = CreateTask("Future task", from.AddDays(2));
        var overdueIncompleteTask = CreateTask("Overdue incomplete task", from.AddDays(-2));
        var overdueCompletedTask = CreateTask("Overdue completed task", from.AddDays(-2));
        overdueCompletedTask.Completed = true;

        using var createResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { currentTask, futureTask, overdueIncompleteTask, overdueCompletedTask });
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        using var getResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));
        var loadedTasks = await ReadTasksAsync(getResponse);

        Assert.Contains(loadedTasks, task => task.Uid == currentTask.Uid);
        Assert.Contains(loadedTasks, task => task.Uid == futureTask.Uid);
        Assert.Contains(loadedTasks, task => task.Uid == overdueIncompleteTask.Uid);
        Assert.DoesNotContain(loadedTasks, task => task.Uid == overdueCompletedTask.Uid);
    }

    [Fact]
    public async Task Tasks_UsersAreIsolatedAndForeignSaveIsIgnored()
    {
        await using var owner = await CreateUserClientAsync();
        await using var foreignUser = await CreateUserClientAsync();
        var from = DateTime.UtcNow.Date;
        var ownerTask = CreateTask("Owner task", from);
        var foreignTask = CreateTask("Foreign task", from);

        using var createResponse = await owner.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { ownerTask });
        var createdOwnerTask = Assert.Single(await ReadTasksAsync(createResponse));

        using var createForeignResponse = await foreignUser.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { foreignTask });
        var createdForeignTask = Assert.Single(await ReadTasksAsync(createForeignResponse));

        using var foreignGetResponse = await foreignUser.HttpClient.GetAsync(CreateTasksUri(from));
        var foreignTasks = await ReadTasksAsync(foreignGetResponse);
        Assert.DoesNotContain(foreignTasks, item => item.Uid == createdOwnerTask.Uid);
        Assert.Contains(foreignTasks, item => item.Uid == createdForeignTask.Uid);

        using var ownerGetResponse = await owner.HttpClient.GetAsync(CreateTasksUri(from));
        var ownerTasks = await ReadTasksAsync(ownerGetResponse);
        Assert.Contains(ownerTasks, item => item.Uid == createdOwnerTask.Uid);
        Assert.DoesNotContain(ownerTasks, item => item.Uid == createdForeignTask.Uid);

        createdOwnerTask.Title = "Foreign update";
        using var foreignSaveResponse = await foreignUser.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { createdOwnerTask });
        Assert.Equal(HttpStatusCode.OK, foreignSaveResponse.StatusCode);
        Assert.Empty(await ReadTasksAsync(foreignSaveResponse));

        using var ownerGetAfterSaveResponse = await owner.HttpClient.GetAsync(CreateTasksUri(from));
        var ownerTasksAfterSave = await ReadTasksAsync(ownerGetAfterSaveResponse);
        var unchangedTask = Assert.Single(
            ownerTasksAfterSave,
            item => item.Uid == createdOwnerTask.Uid);
        Assert.Equal("Owner task", unchangedTask.Title);
        Assert.Equal(1, unchangedTask.Version);
    }

    [Fact]
    public async Task Tasks_StaleVersionIsIgnoredAndWinningValueRemains()
    {
        await using var user = await CreateUserClientAsync();
        var from = DateTime.UtcNow.Date;
        var task = CreateTask("Original title", from);

        using var createResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { task });
        var createdTask = Assert.Single(await ReadTasksAsync(createResponse));
        var staleTask = CopyTask(createdTask);
        var winningTask = CopyTask(createdTask);
        winningTask.Title = "Winning title";

        using var updateResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { winningTask });
        var updatedTask = Assert.Single(await ReadTasksAsync(updateResponse));
        Assert.Equal("Winning title", updatedTask.Title);
        Assert.Equal(2, updatedTask.Version);

        staleTask.Title = "Stale title";
        using var staleResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { staleTask });
        Assert.Equal(HttpStatusCode.OK, staleResponse.StatusCode);
        Assert.Empty(await ReadTasksAsync(staleResponse));

        using var getResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));
        var loadedTask = Assert.Single(
            await ReadTasksAsync(getResponse),
            item => item.Uid == createdTask.Uid);
        Assert.Equal("Winning title", loadedTask.Title);
        Assert.Equal(2, loadedTask.Version);
    }

    [Fact]
    public async Task Tasks_BatchReorder_IncrementsEveryVersion()
    {
        await using var user = await CreateUserClientAsync();
        var from = DateTime.UtcNow.Date;
        var tasks = Enumerable.Range(0, 3)
            .Select(index => CreateTask($"Reorder task {index}", from, index))
            .ToArray();

        using var createResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            tasks);
        var createdTasks = await ReadTasksAsync(createResponse);
        Assert.Equal(3, createdTasks.Length);

        var expectedOrders = new Dictionary<string, int>
        {
            [createdTasks[0].Uid] = 30,
            [createdTasks[1].Uid] = 10,
            [createdTasks[2].Uid] = 20,
        };
        foreach (var task in createdTasks)
        {
            task.Order = expectedOrders[task.Uid];
        }

        using var reorderResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            createdTasks);
        var reorderedTasks = await ReadTasksAsync(reorderResponse);
        Assert.Equal(3, reorderedTasks.Length);
        Assert.All(reorderedTasks, task => Assert.Equal(2, task.Version));

        using var getResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));
        var loadedTasks = await ReadTasksAsync(getResponse);
        foreach (var task in loadedTasks.Where(item => expectedOrders.ContainsKey(item.Uid)))
        {
            Assert.Equal(expectedOrders[task.Uid], task.Order);
            Assert.Equal(2, task.Version);
        }
    }

    [Fact]
    public async Task Tasks_IndependentUserFlows_RunConcurrently()
    {
        await using var firstUser = await CreateUserClientAsync();
        await using var secondUser = await CreateUserClientAsync();

        await Task.WhenAll(
            RunIndependentUserFlowAsync(firstUser, "first"),
            RunIndependentUserFlowAsync(secondUser, "second"));
    }

    private static async Task RunIndependentUserFlowAsync(TestUserClient user, string name)
    {
        var from = DateTime.UtcNow.Date;
        var task = CreateTask($"{name} original", from);

        using var createResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { task });
        var createdTask = Assert.Single(await ReadTasksAsync(createResponse));

        createdTask.Title = $"{name} updated";
        createdTask.Order = 99;
        using var updateResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { createdTask });
        var updatedTask = Assert.Single(await ReadTasksAsync(updateResponse));
        Assert.Equal($"{name} updated", updatedTask.Title);
        Assert.Equal(99, updatedTask.Order);
        Assert.Equal(2, updatedTask.Version);

        using var getResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));
        var loadedTask = Assert.Single(
            await ReadTasksAsync(getResponse),
            item => item.Uid == task.Uid);
        Assert.Equal($"{name} updated", loadedTask.Title);
        Assert.Equal(2, loadedTask.Version);
    }

    private static TaskDto CreateTask(string title, DateTime date, int order = 0)
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

    private static TaskDto CopyTask(TaskDto task)
    {
        return new TaskDto
        {
            Uid = task.Uid,
            Date = task.Date,
            Time = task.Time,
            Title = task.Title,
            Order = task.Order,
            Completed = task.Completed,
            IsProbable = task.IsProbable,
            Deleted = task.Deleted,
            Type = task.Type,
            Version = task.Version,
        };
    }
}
