using System.Net;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Infrastructure;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration;

public sealed class TasksIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task Tasks_CreateNoDateAndDated_ReturnPublicFields()
    {
        await using var user = await CreateUserClientAsync();
        var from = DateTime.UtcNow.Date;
        var datedDate = from.AddDays(1);
        var noDate = new TaskDto
        {
            Uid = CreateUniqueTaskUid(),
            Title = "No date task",
            Time = 615,
            Order = 4,
            Type = TaskTypeDto.Additional,
            IsProbable = true,
        };
        var dated = new TaskDto
        {
            Uid = CreateUniqueTaskUid(),
            Title = "Dated task",
            Date = datedDate,
            Time = 1050,
            Order = 8,
            Type = TaskTypeDto.Weekly,
        };

        using var saveResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { noDate, dated });

        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        var savedTasks = await ReadTasksAsync(saveResponse);
        Assert.Equal(2, savedTasks.Length);

        var savedNoDate = Assert.Single(savedTasks, task => task.Uid == noDate.Uid);
        AssertTaskFields(savedNoDate, noDate, expectedVersion: 1);
        Assert.Null(savedNoDate.Date);

        var savedDated = Assert.Single(savedTasks, task => task.Uid == dated.Uid);
        AssertTaskFields(savedDated, dated, expectedVersion: 1);
        Assert.Equal(datedDate, savedDated.Date);

        using var getResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var loadedTasks = await ReadTasksAsync(getResponse);
        Assert.Equal(2, loadedTasks.Length);
        AssertTaskFields(
            Assert.Single(loadedTasks, task => task.Uid == noDate.Uid),
            noDate,
            expectedVersion: 1);
        AssertTaskFields(
            Assert.Single(loadedTasks, task => task.Uid == dated.Uid),
            dated,
            expectedVersion: 1);
    }

    [Fact]
    public async Task Tasks_UpdateAndDelete_IncrementVersionAndExpireDeletedNoDateTask()
    {
        await using var user = await CreateUserClientAsync();
        var from = DateTime.UtcNow.Date;
        var task = new TaskDto
        {
            Uid = CreateUniqueTaskUid(),
            Title = "Task before update",
            Order = 10,
            Type = TaskTypeDto.Simple,
        };

        using var createResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { task });
        var createdTask = Assert.Single(await ReadTasksAsync(createResponse));
        Assert.Equal(1, createdTask.Version);

        createdTask.Title = "Task after update";
        createdTask.Order = 2;
        using var updateResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { createdTask });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedTask = Assert.Single(await ReadTasksAsync(updateResponse));
        Assert.Equal(createdTask.Uid, updatedTask.Uid);
        Assert.Equal("Task after update", updatedTask.Title);
        Assert.Equal(2, updatedTask.Order);
        Assert.Equal(2, updatedTask.Version);
        Assert.False(updatedTask.Deleted);

        using var updatedGetResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));
        var loadedUpdatedTask = Assert.Single(
            await ReadTasksAsync(updatedGetResponse),
            item => item.Uid == task.Uid);
        Assert.Equal("Task after update", loadedUpdatedTask.Title);
        Assert.Equal(2, loadedUpdatedTask.Order);
        Assert.Equal(2, loadedUpdatedTask.Version);

        updatedTask.Deleted = true;
        using var deleteResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { updatedTask });

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deletedTask = Assert.Single(await ReadTasksAsync(deleteResponse));
        Assert.Equal(task.Uid, deletedTask.Uid);
        Assert.True(deletedTask.Deleted);
        Assert.Equal(3, deletedTask.Version);

        using var deletedGetResponse = await user.HttpClient.GetAsync(CreateTasksUri(from));
        var loadedDeletedTask = Assert.Single(
            await ReadTasksAsync(deletedGetResponse),
            item => item.Uid == task.Uid);
        Assert.True(loadedDeletedTask.Deleted);
        Assert.Equal(3, loadedDeletedTask.Version);

        using var expiredGetResponse = await user.HttpClient.GetAsync(
            CreateTasksUri(from.AddDays(8)));
        var expiredTasks = await ReadTasksAsync(expiredGetResponse);
        Assert.DoesNotContain(expiredTasks, item => item.Uid == task.Uid);
    }

    private static void AssertTaskFields(TaskDto actual, TaskDto expected, int expectedVersion)
    {
        Assert.Equal(expected.Uid, actual.Uid);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Date, actual.Date);
        Assert.Equal(expected.Time, actual.Time);
        Assert.Equal(expected.Type, actual.Type);
        Assert.Equal(expected.IsProbable, actual.IsProbable);
        Assert.Equal(expected.Order, actual.Order);
        Assert.Equal(expectedVersion, actual.Version);
        Assert.False(actual.Deleted);
    }
}
