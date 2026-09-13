using System.Net;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;
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

        using var saveResponse = await TasksApi.SaveAsync(
            user.HttpClient,
            [noDate, dated]);

        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        var savedTasks = await TasksReader.ReadAsync(saveResponse);
        Assert.Equal(2, savedTasks.Length);

        var savedNoDate = Assert.Single(savedTasks, task => task.Uid == noDate.Uid);
        AssertTaskFields(savedNoDate, noDate, expectedVersion: 1);
        Assert.Null(savedNoDate.Date);

        var savedDated = Assert.Single(savedTasks, task => task.Uid == dated.Uid);
        AssertTaskFields(savedDated, dated, expectedVersion: 1);
        Assert.Equal(datedDate, savedDated.Date);

        using var getResponse = await TasksApi.LoadAsync(user.HttpClient, from);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var loadedTasks = await TasksReader.ReadAsync(getResponse);
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

        using var createResponse = await TasksApi.SaveAsync(user.HttpClient, [task]);
        var createdTask = Assert.Single(await TasksReader.ReadAsync(createResponse));
        Assert.Equal(1, createdTask.Version);

        createdTask.Title = "Task after update";
        createdTask.Order = 2;
        using var updateResponse = await TasksApi.SaveAsync(
            user.HttpClient,
            [createdTask]);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedTask = Assert.Single(await TasksReader.ReadAsync(updateResponse));
        Assert.Equal(createdTask.Uid, updatedTask.Uid);
        Assert.Equal("Task after update", updatedTask.Title);
        Assert.Equal(2, updatedTask.Order);
        Assert.Equal(2, updatedTask.Version);
        Assert.False(updatedTask.Deleted);

        using var updatedGetResponse = await TasksApi.LoadAsync(user.HttpClient, from);
        var loadedUpdatedTask = Assert.Single(
            await TasksReader.ReadAsync(updatedGetResponse),
            item => item.Uid == task.Uid);
        Assert.Equal("Task after update", loadedUpdatedTask.Title);
        Assert.Equal(2, loadedUpdatedTask.Order);
        Assert.Equal(2, loadedUpdatedTask.Version);

        updatedTask.Deleted = true;
        using var deleteResponse = await TasksApi.SaveAsync(
            user.HttpClient,
            [updatedTask]);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        var deletedTask = Assert.Single(await TasksReader.ReadAsync(deleteResponse));
        Assert.Equal(task.Uid, deletedTask.Uid);
        Assert.True(deletedTask.Deleted);
        Assert.Equal(3, deletedTask.Version);

        using var deletedGetResponse = await TasksApi.LoadAsync(user.HttpClient, from);
        var loadedDeletedTask = Assert.Single(
            await TasksReader.ReadAsync(deletedGetResponse),
            item => item.Uid == task.Uid);
        Assert.True(loadedDeletedTask.Deleted);
        Assert.Equal(3, loadedDeletedTask.Version);

        using var expiredGetResponse = await TasksApi.LoadAsync(
            user.HttpClient,
            from.AddDays(8));
        var expiredTasks = await TasksReader.ReadAsync(expiredGetResponse);
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
