using System.Net;
using System.Net.Http.Json;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;
using static DD.Tests.Integration.Helpers.RecurrencesHelper;

namespace DD.Tests.Integration;

public sealed class RecurrencesIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task Recurrences_LifecycleThroughCreateUpdateDeleteRoundTrip_ReturnsPersistedContract()
    {
        await using var user = await CreateUserClientAsync();
        var today = IntegrationTestClock.UtcToday;

        using var emptyGetResponse = await user.HttpClient.GetAsync(RecurrencesUri);
        Assert.Equal(HttpStatusCode.OK, emptyGetResponse.StatusCode);
        Assert.Empty(await ReadRecurrencesAsync(emptyGetResponse));

        var recurrence = CreateRecurrence(
            "Weekly task",
            today,
            today.AddDays(30),
            everyNthDay: 3,
            everyWeekday: RecurrenceWeekday.Monday | RecurrenceWeekday.Wednesday,
            everyMonthDay: "1,15");

        using var createResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { recurrence });
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Equal(1, await ReadRecurrenceCountAsync(createResponse));

        using var afterCreateGetResponse = await user.HttpClient.GetAsync(RecurrencesUri);
        var loaded = Assert.Single(
            await ReadRecurrencesAsync(afterCreateGetResponse),
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
        using var noopResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { loaded });
        Assert.Equal(HttpStatusCode.OK, noopResponse.StatusCode);
        Assert.Equal(0, await ReadRecurrenceCountAsync(noopResponse));

        loaded.Task = "Updated weekly task";
        loaded.StartDate = today.AddDays(1);
        loaded.EndDate = today.AddDays(40);
        loaded.EveryNthDay = 5;
        loaded.EveryWeekday = RecurrenceWeekday.Tuesday | RecurrenceWeekday.Thursday;
        loaded.EveryMonthDay = "2,16";
        using var updateResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { loaded });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal(1, await ReadRecurrenceCountAsync(updateResponse));

        using var afterUpdateGetResponse = await user.HttpClient.GetAsync(RecurrencesUri);
        var updatedLoaded = Assert.Single(
            await ReadRecurrencesAsync(afterUpdateGetResponse),
            item => item.Uid == recurrence.Uid);
        Assert.Equal("Updated weekly task", updatedLoaded.Task);
        Assert.Equal(today.AddDays(1), updatedLoaded.StartDate);
        Assert.Equal(today.AddDays(40), updatedLoaded.EndDate);
        Assert.Equal(5, updatedLoaded.EveryNthDay);
        Assert.Equal(RecurrenceWeekday.Tuesday | RecurrenceWeekday.Thursday, updatedLoaded.EveryWeekday);
        Assert.Equal("2,16", updatedLoaded.EveryMonthDay);

        var secondRecurrence = CreateRecurrence("Second recurrence", today);
        using var secondCreateResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { secondRecurrence });
        Assert.Equal(1, await ReadRecurrenceCountAsync(secondCreateResponse));

        updatedLoaded.IsDeleted = true;
        using var deleteResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { updatedLoaded });
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(1, await ReadRecurrenceCountAsync(deleteResponse));

        using var afterDeleteGetResponse = await user.HttpClient.GetAsync(RecurrencesUri);
        var remaining = await ReadRecurrencesAsync(afterDeleteGetResponse);
        Assert.DoesNotContain(remaining, item => item.Uid == recurrence.Uid);
        Assert.Contains(remaining, item => item.Uid == secondRecurrence.Uid);
    }

    [Fact]
    public async Task Recurrences_ForeignUid_ReturnsInternalServerErrorAndOwnerRecurrenceIsUnchanged()
    {
        await using var owner = await CreateUserClientAsync();
        await using var foreignUser = await CreateUserClientAsync();
        var today = IntegrationTestClock.UtcToday;
        var ownerRecurrence = CreateRecurrence("Owner recurrence", today);

        using var createResponse = await owner.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { ownerRecurrence });
        Assert.Equal(1, await ReadRecurrenceCountAsync(createResponse));

        ownerRecurrence.Task = "Foreign update attempt";
        using var foreignResponse = await foreignUser.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { ownerRecurrence });

        Assert.Equal(HttpStatusCode.InternalServerError, foreignResponse.StatusCode);
        var problemDetails = await foreignResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("An unexpected error occurred", problemDetails.Title);

        using var ownerGetResponse = await owner.HttpClient.GetAsync(RecurrencesUri);
        var unchanged = Assert.Single(
            await ReadRecurrencesAsync(ownerGetResponse),
            item => item.Uid == ownerRecurrence.Uid);
        Assert.Equal("Owner recurrence", unchanged.Task);
    }

    [Fact]
    public async Task RecurrencesCreate_ConcurrentAndRepeatedCalls_GenerateExactlyOneTaskAndNoScheduleRecurrenceCreatesNone()
    {
        await using var user = await CreateUserClientAsync();
        var today = IntegrationTestClock.UtcToday;
        var scheduledTitle = $"Recurring task {CreateUniqueRecurrenceUid()}";
        var scheduledRecurrence = CreateRecurrence(scheduledTitle, today, today, everyNthDay: 1);

        using var seedResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { scheduledRecurrence });
        Assert.Equal(1, await ReadRecurrenceCountAsync(seedResponse));

        var concurrentResults = await Task.WhenAll(
            CreateRecurrencesAsync(user.HttpClient),
            CreateRecurrencesAsync(user.HttpClient),
            CreateRecurrencesAsync(user.HttpClient));
        var repeatedResult = await CreateRecurrencesAsync(user.HttpClient);

        Assert.Equal(1, concurrentResults.Sum() + repeatedResult);

        using var tasksResponse = await user.HttpClient.GetAsync(CreateTasksUri(today));
        var tasks = await ReadTasksAsync(tasksResponse);
        Assert.Single(tasks, task => task.Title == scheduledTitle);

        var noScheduleTitle = $"No schedule task {CreateUniqueRecurrenceUid()}";
        var noScheduleRecurrence = CreateRecurrence(noScheduleTitle, today);
        using var noScheduleSeedResponse = await user.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { noScheduleRecurrence });
        Assert.Equal(1, await ReadRecurrenceCountAsync(noScheduleSeedResponse));

        var afterNoScheduleResult = await CreateRecurrencesAsync(user.HttpClient);
        Assert.Equal(0, afterNoScheduleResult);

        using var finalTasksResponse = await user.HttpClient.GetAsync(CreateTasksUri(today));
        var finalTasks = await ReadTasksAsync(finalTasksResponse);
        Assert.Single(finalTasks, task => task.Title == scheduledTitle);
        Assert.DoesNotContain(finalTasks, task => task.Title == noScheduleTitle);
    }

    [Fact]
    public async Task Recurrences_TwoUsers_LoadAndGenerateOnlyOwnRecurrences()
    {
        await using var firstUser = await CreateUserClientAsync();
        await using var secondUser = await CreateUserClientAsync();
        var today = IntegrationTestClock.UtcToday;
        var firstTitle = $"First recurrence {CreateUniqueRecurrenceUid()}";
        var secondTitle = $"Second recurrence {CreateUniqueRecurrenceUid()}";
        var firstRecurrence = CreateRecurrence(firstTitle, today, today, everyNthDay: 1);
        var secondRecurrence = CreateRecurrence(secondTitle, today, today, everyNthDay: 1);

        using var firstSeedResponse = await firstUser.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { firstRecurrence });
        Assert.Equal(1, await ReadRecurrenceCountAsync(firstSeedResponse));
        using var secondSeedResponse = await secondUser.HttpClient.PostAsJsonAsync(
            RecurrencesRoute,
            new[] { secondRecurrence });
        Assert.Equal(1, await ReadRecurrenceCountAsync(secondSeedResponse));

        using var firstLoadResponse = await firstUser.HttpClient.GetAsync(RecurrencesUri);
        var firstLoaded = Assert.Single(await ReadRecurrencesAsync(firstLoadResponse));
        Assert.Equal(firstRecurrence.Uid, firstLoaded.Uid);
        using var secondLoadResponse = await secondUser.HttpClient.GetAsync(RecurrencesUri);
        var secondLoaded = Assert.Single(await ReadRecurrencesAsync(secondLoadResponse));
        Assert.Equal(secondRecurrence.Uid, secondLoaded.Uid);

        Assert.Equal(1, await CreateRecurrencesAsync(firstUser.HttpClient));
        using var firstTasksResponse = await firstUser.HttpClient.GetAsync(CreateTasksUri(today));
        var firstTasks = await ReadTasksAsync(firstTasksResponse);
        Assert.Single(firstTasks, task => task.Title == firstTitle);
        Assert.DoesNotContain(firstTasks, task => task.Title == secondTitle);

        using var secondTasksResponse = await secondUser.HttpClient.GetAsync(CreateTasksUri(today));
        var secondTasks = await ReadTasksAsync(secondTasksResponse);
        Assert.DoesNotContain(secondTasks, task => task.Title == firstTitle);
        Assert.DoesNotContain(secondTasks, task => task.Title == secondTitle);
    }
}
