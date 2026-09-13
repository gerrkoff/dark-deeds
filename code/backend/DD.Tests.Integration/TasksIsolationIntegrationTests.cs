using System.Net;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Clients;
using DD.Tests.Integration.Infrastructure.Readers;
using Xunit;

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

        using var createResponse = await TasksApi.SaveAsync(
            user.HttpClient,
            [currentTask, futureTask, overdueIncompleteTask, overdueCompletedTask]);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        using var getResponse = await TasksApi.LoadAsync(user.HttpClient, from);
        var loadedTasks = await TasksReader.ReadAsync(getResponse);

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

        using var createResponse = await TasksApi.SaveAsync(owner.HttpClient, [ownerTask]);
        var createdOwnerTask = Assert.Single(await TasksReader.ReadAsync(createResponse));

        using var createForeignResponse = await TasksApi.SaveAsync(
            foreignUser.HttpClient,
            [foreignTask]);
        var createdForeignTask = Assert.Single(
            await TasksReader.ReadAsync(createForeignResponse));

        using var foreignGetResponse = await TasksApi.LoadAsync(foreignUser.HttpClient, from);
        var foreignTasks = await TasksReader.ReadAsync(foreignGetResponse);
        Assert.DoesNotContain(foreignTasks, item => item.Uid == createdOwnerTask.Uid);
        Assert.Contains(foreignTasks, item => item.Uid == createdForeignTask.Uid);

        using var ownerGetResponse = await TasksApi.LoadAsync(owner.HttpClient, from);
        var ownerTasks = await TasksReader.ReadAsync(ownerGetResponse);
        Assert.Contains(ownerTasks, item => item.Uid == createdOwnerTask.Uid);
        Assert.DoesNotContain(ownerTasks, item => item.Uid == createdForeignTask.Uid);

        createdOwnerTask.Title = "Foreign update";
        using var foreignSaveResponse = await TasksApi.SaveAsync(
            foreignUser.HttpClient,
            [createdOwnerTask]);
        Assert.Equal(HttpStatusCode.OK, foreignSaveResponse.StatusCode);
        Assert.Empty(await TasksReader.ReadAsync(foreignSaveResponse));

        using var ownerGetAfterSaveResponse = await TasksApi.LoadAsync(owner.HttpClient, from);
        var ownerTasksAfterSave = await TasksReader.ReadAsync(ownerGetAfterSaveResponse);
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

        using var createResponse = await TasksApi.SaveAsync(user.HttpClient, [task]);
        var createdTask = Assert.Single(await TasksReader.ReadAsync(createResponse));
        var staleTask = CopyTask(createdTask);
        var winningTask = CopyTask(createdTask);
        winningTask.Title = "Winning title";

        using var updateResponse = await TasksApi.SaveAsync(user.HttpClient, [winningTask]);
        var updatedTask = Assert.Single(await TasksReader.ReadAsync(updateResponse));
        Assert.Equal("Winning title", updatedTask.Title);
        Assert.Equal(2, updatedTask.Version);

        staleTask.Title = "Stale title";
        using var staleResponse = await TasksApi.SaveAsync(user.HttpClient, [staleTask]);
        Assert.Equal(HttpStatusCode.OK, staleResponse.StatusCode);
        Assert.Empty(await TasksReader.ReadAsync(staleResponse));

        using var getResponse = await TasksApi.LoadAsync(user.HttpClient, from);
        var loadedTask = Assert.Single(
            await TasksReader.ReadAsync(getResponse),
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

        using var createResponse = await TasksApi.SaveAsync(user.HttpClient, tasks);
        var createdTasks = await TasksReader.ReadAsync(createResponse);
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

        using var reorderResponse = await TasksApi.SaveAsync(user.HttpClient, createdTasks);
        var reorderedTasks = await TasksReader.ReadAsync(reorderResponse);
        Assert.Equal(3, reorderedTasks.Length);
        Assert.All(reorderedTasks, task => Assert.Equal(2, task.Version));

        using var getResponse = await TasksApi.LoadAsync(user.HttpClient, from);
        var loadedTasks = await TasksReader.ReadAsync(getResponse);
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

        using var createResponse = await TasksApi.SaveAsync(user.HttpClient, [task]);
        var createdTask = Assert.Single(await TasksReader.ReadAsync(createResponse));

        createdTask.Title = $"{name} updated";
        createdTask.Order = 99;
        using var updateResponse = await TasksApi.SaveAsync(user.HttpClient, [createdTask]);
        var updatedTask = Assert.Single(await TasksReader.ReadAsync(updateResponse));
        Assert.Equal($"{name} updated", updatedTask.Title);
        Assert.Equal(99, updatedTask.Order);
        Assert.Equal(2, updatedTask.Version);

        using var getResponse = await TasksApi.LoadAsync(user.HttpClient, from);
        var loadedTask = Assert.Single(
            await TasksReader.ReadAsync(getResponse),
            item => item.Uid == task.Uid);
        Assert.Equal($"{name} updated", loadedTask.Title);
        Assert.Equal(2, loadedTask.Version);
    }

    private static TaskDto CreateTask(string title, DateTime date, int order = 0)
    {
        return new TaskDto
        {
            Uid = TasksHelper.CreateUniqueTaskUid(),
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
