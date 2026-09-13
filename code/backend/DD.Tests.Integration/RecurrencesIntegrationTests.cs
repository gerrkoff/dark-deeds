using System.Net;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.ExternalDependencies;
using DD.Tests.Integration.Infrastructure.Readers;
using Xunit;

namespace DD.Tests.Integration;

public sealed class RecurrencesIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task Recurrences_LifecycleThroughCreateUpdateDeleteRoundTrip_ReturnsPersistedContract()
    {
        await using var user = await CreateUserClientAsync();
        var today = TestDateService.UtcToday;

        using var emptyGetResponse = await RecurrencesApi.LoadAsync(user.HttpClient);
        Assert.Equal(HttpStatusCode.OK, emptyGetResponse.StatusCode);
        Assert.Empty(await RecurrencesReader.ReadAsync(emptyGetResponse));

        var recurrence = RecurrencesHelper.CreateRecurrence(
            "Weekly task",
            today,
            today.AddDays(30),
            everyNthDay: 3,
            everyWeekday: RecurrenceWeekday.Monday | RecurrenceWeekday.Wednesday,
            everyMonthDay: "1,15");

        using var createResponse = await RecurrencesApi.SaveAsync(
            user.HttpClient,
            [recurrence]);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(createResponse));

        using var afterCreateGetResponse = await RecurrencesApi.LoadAsync(user.HttpClient);
        var loaded = Assert.Single(
            await RecurrencesReader.ReadAsync(afterCreateGetResponse),
            item => item.Uid == recurrence.Uid);
        Assert.Equal(recurrence.Task, loaded.Task);
        Assert.Equal(DateTimeKind.Utc, loaded.StartDate.Kind);
        Assert.Equal(today, loaded.StartDate);
        Assert.Equal(DateTimeKind.Utc, loaded.EndDate!.Value.Kind);
        Assert.Equal(today.AddDays(30), loaded.EndDate);
        Assert.Equal(3, loaded.EveryNthDay);
        Assert.Equal(RecurrenceWeekday.Monday | RecurrenceWeekday.Wednesday, loaded.EveryWeekday);
        Assert.Equal("1,15", loaded.EveryMonthDay);
        Assert.False(loaded.IsDeleted);

        // Re-posting the unchanged loaded snapshot must not count as an update.
        using var noopResponse = await RecurrencesApi.SaveAsync(user.HttpClient, [loaded]);
        Assert.Equal(HttpStatusCode.OK, noopResponse.StatusCode);
        Assert.Equal(0, await RecurrencesReader.ReadCountAsync(noopResponse));

        loaded.Task = "Updated weekly task";
        loaded.StartDate = today.AddDays(1);
        loaded.EndDate = today.AddDays(40);
        loaded.EveryNthDay = 5;
        loaded.EveryWeekday = RecurrenceWeekday.Tuesday | RecurrenceWeekday.Thursday;
        loaded.EveryMonthDay = "2,16";
        using var updateResponse = await RecurrencesApi.SaveAsync(user.HttpClient, [loaded]);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(updateResponse));

        using var afterUpdateGetResponse = await RecurrencesApi.LoadAsync(user.HttpClient);
        var updatedLoaded = Assert.Single(
            await RecurrencesReader.ReadAsync(afterUpdateGetResponse),
            item => item.Uid == recurrence.Uid);
        Assert.Equal("Updated weekly task", updatedLoaded.Task);
        Assert.Equal(today.AddDays(1), updatedLoaded.StartDate);
        Assert.Equal(today.AddDays(40), updatedLoaded.EndDate);
        Assert.Equal(5, updatedLoaded.EveryNthDay);
        Assert.Equal(RecurrenceWeekday.Tuesday | RecurrenceWeekday.Thursday, updatedLoaded.EveryWeekday);
        Assert.Equal("2,16", updatedLoaded.EveryMonthDay);

        var secondRecurrence = RecurrencesHelper.CreateRecurrence(
            "Second recurrence",
            today);
        using var secondCreateResponse = await RecurrencesApi.SaveAsync(
            user.HttpClient,
            [secondRecurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(secondCreateResponse));

        updatedLoaded.IsDeleted = true;
        using var deleteResponse = await RecurrencesApi.SaveAsync(
            user.HttpClient,
            [updatedLoaded]);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(deleteResponse));

        using var afterDeleteGetResponse = await RecurrencesApi.LoadAsync(user.HttpClient);
        var remaining = await RecurrencesReader.ReadAsync(afterDeleteGetResponse);
        Assert.DoesNotContain(remaining, item => item.Uid == recurrence.Uid);
        Assert.Contains(remaining, item => item.Uid == secondRecurrence.Uid);
    }

    [Fact]
    public async Task Recurrences_ForeignUid_ReturnsInternalServerErrorAndOwnerRecurrenceIsUnchanged()
    {
        await using var owner = await CreateUserClientAsync();
        await using var foreignUser = await CreateUserClientAsync();
        var today = TestDateService.UtcToday;
        var ownerRecurrence = RecurrencesHelper.CreateRecurrence(
            "Owner recurrence",
            today);

        using var createResponse = await RecurrencesApi.SaveAsync(
            owner.HttpClient,
            [ownerRecurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(createResponse));

        ownerRecurrence.Task = "Foreign update attempt";
        using var foreignResponse = await RecurrencesApi.SaveAsync(
            foreignUser.HttpClient,
            [ownerRecurrence]);

        Assert.Equal(HttpStatusCode.InternalServerError, foreignResponse.StatusCode);
        var problemDetails = await ProblemDetailsReader.ReadAsync(foreignResponse);
        Assert.Equal("An unexpected error occurred", problemDetails.Title);

        using var ownerGetResponse = await RecurrencesApi.LoadAsync(owner.HttpClient);
        var unchanged = Assert.Single(
            await RecurrencesReader.ReadAsync(ownerGetResponse),
            item => item.Uid == ownerRecurrence.Uid);
        Assert.Equal("Owner recurrence", unchanged.Task);
    }

    [Fact]
    public async Task RecurrencesCreate_ConcurrentAndRepeatedCalls_GenerateExactlyOneTaskAndNoScheduleRecurrenceCreatesNone()
    {
        await using var user = await CreateUserClientAsync();
        var today = TestDateService.UtcToday;
        var scheduledTitle =
            $"Recurring task {RecurrencesHelper.CreateUniqueRecurrenceUid()}";
        var scheduledRecurrence = RecurrencesHelper.CreateRecurrence(
            scheduledTitle,
            today,
            today,
            everyNthDay: 1);

        using var seedResponse = await RecurrencesApi.SaveAsync(
            user.HttpClient,
            [scheduledRecurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(seedResponse));

        var concurrentResults = await Task.WhenAll(
            CreateRecurrencesAsync(user.HttpClient),
            CreateRecurrencesAsync(user.HttpClient),
            CreateRecurrencesAsync(user.HttpClient));
        var repeatedResult = await CreateRecurrencesAsync(user.HttpClient);

        Assert.Equal(1, concurrentResults.Sum() + repeatedResult);

        using var tasksResponse = await TasksApi.LoadAsync(user.HttpClient, today);
        var tasks = await TasksReader.ReadAsync(tasksResponse);
        var generatedTask = Assert.Single(tasks);
        Assert.Equal(scheduledTitle, generatedTask.Title);

        var noScheduleTitle =
            $"No schedule task {RecurrencesHelper.CreateUniqueRecurrenceUid()}";
        var noScheduleRecurrence = RecurrencesHelper.CreateRecurrence(
            noScheduleTitle,
            today);
        using var noScheduleSeedResponse = await RecurrencesApi.SaveAsync(
            user.HttpClient,
            [noScheduleRecurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(noScheduleSeedResponse));

        var afterNoScheduleResult = await CreateRecurrencesAsync(user.HttpClient);
        Assert.Equal(0, afterNoScheduleResult);

        using var finalTasksResponse = await TasksApi.LoadAsync(user.HttpClient, today);
        var finalTasks = await TasksReader.ReadAsync(finalTasksResponse);
        var remainingTask = Assert.Single(finalTasks);
        Assert.Equal(scheduledTitle, remainingTask.Title);
    }

    [Fact]
    public async Task Recurrences_TwoUsers_LoadAndGenerateOnlyOwnRecurrences()
    {
        await using var firstUser = await CreateUserClientAsync();
        await using var secondUser = await CreateUserClientAsync();
        var today = TestDateService.UtcToday;
        var firstTitle =
            $"First recurrence {RecurrencesHelper.CreateUniqueRecurrenceUid()}";
        var secondTitle =
            $"Second recurrence {RecurrencesHelper.CreateUniqueRecurrenceUid()}";
        var firstRecurrence = RecurrencesHelper.CreateRecurrence(
            firstTitle,
            today,
            today,
            everyNthDay: 1);
        var secondRecurrence = RecurrencesHelper.CreateRecurrence(
            secondTitle,
            today,
            today,
            everyNthDay: 1);

        using var firstSeedResponse = await RecurrencesApi.SaveAsync(
            firstUser.HttpClient,
            [firstRecurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(firstSeedResponse));
        using var secondSeedResponse = await RecurrencesApi.SaveAsync(
            secondUser.HttpClient,
            [secondRecurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(secondSeedResponse));

        using var firstLoadResponse = await RecurrencesApi.LoadAsync(firstUser.HttpClient);
        var firstLoaded = Assert.Single(await RecurrencesReader.ReadAsync(firstLoadResponse));
        Assert.Equal(firstRecurrence.Uid, firstLoaded.Uid);
        using var secondLoadResponse = await RecurrencesApi.LoadAsync(secondUser.HttpClient);
        var secondLoaded = Assert.Single(await RecurrencesReader.ReadAsync(secondLoadResponse));
        Assert.Equal(secondRecurrence.Uid, secondLoaded.Uid);

        Assert.Equal(1, await CreateRecurrencesAsync(firstUser.HttpClient));
        using var firstTasksResponse = await TasksApi.LoadAsync(firstUser.HttpClient, today);
        var firstTasks = await TasksReader.ReadAsync(firstTasksResponse);
        Assert.Single(firstTasks, task => task.Title == firstTitle);
        Assert.DoesNotContain(firstTasks, task => task.Title == secondTitle);

        using var secondTasksResponse = await TasksApi.LoadAsync(secondUser.HttpClient, today);
        var secondTasks = await TasksReader.ReadAsync(secondTasksResponse);
        Assert.DoesNotContain(secondTasks, task => task.Title == firstTitle);
        Assert.DoesNotContain(secondTasks, task => task.Title == secondTitle);
    }

    private static async Task<int> CreateRecurrencesAsync(HttpClient client)
    {
        using var response = await RecurrencesApi.CreateTasksAsync(client);
        return await RecurrencesReader.ReadCountAsync(response);
    }
}
