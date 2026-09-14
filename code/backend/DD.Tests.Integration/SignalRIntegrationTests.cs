using System.Net;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Clients;
using DD.Tests.Integration.Infrastructure.ExternalDependencies;
using DD.Tests.Integration.Infrastructure.Readers;
using Xunit;

namespace DD.Tests.Integration;

public sealed class SignalRIntegrationTests : IntegrationTestBase
{
    private static readonly TimeSpan UpdateTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task TaskHub_AnonymousNegotiationIsRejected_AuthenticatedUserCanConnect()
    {
        using var client = await CreateClientAsync();

        using var anonymousResponse = await SignalRApi.NegotiateTaskHubAsync(client);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        await using var user = await CreateUserClientAsync();
        await using var signalRClient = await TestSignalRClient.CreateAsync(
            user,
            "authenticated-client");
    }

    [Fact]
    public async Task TaskHub_UpdatesStayInUserGroup_AndMatchingClientConnectionsAreSuppressed()
    {
        await using var user = await CreateUserClientAsync();
        await using var foreignUser = await CreateUserClientAsync();
        await using var matchingClientOne = await TestSignalRClient.CreateAsync(
            user,
            "same-client");
        await using var matchingClientTwo = await TestSignalRClient.CreateAsync(
            user,
            "same-client");
        await using var differentClient = await TestSignalRClient.CreateAsync(
            user,
            "different-client");
        await using var foreignClient = await TestSignalRClient.CreateAsync(
            foreignUser,
            "foreign-client");

        var target = CreateTask("suppressed target");
        await SaveTaskAsync(user, target, "same-client");

        await differentClient.WaitForTaskAsync(target.Uid, UpdateTimeout);

        var userSentinel = CreateTask("user sentinel");
        await SaveTaskAsync(user, userSentinel);
        await matchingClientOne.WaitForTaskAsync(userSentinel.Uid, UpdateTimeout);
        await matchingClientTwo.WaitForTaskAsync(userSentinel.Uid, UpdateTimeout);
        await differentClient.WaitForTaskAsync(userSentinel.Uid, UpdateTimeout);

        Assert.False(matchingClientOne.HasReceived(target.Uid));
        Assert.False(matchingClientTwo.HasReceived(target.Uid));
        Assert.True(differentClient.HasReceived(target.Uid));

        var foreignSentinel = CreateTask("foreign sentinel");
        await SaveTaskAsync(foreignUser, foreignSentinel);
        await foreignClient.WaitForTaskAsync(foreignSentinel.Uid, UpdateTimeout);

        Assert.False(foreignClient.HasReceived(target.Uid));
        Assert.False(foreignClient.HasReceived(userSentinel.Uid));

        var postForeignUserSentinel = CreateTask("post-foreign user sentinel");
        await SaveTaskAsync(user, postForeignUserSentinel);
        await matchingClientOne.WaitForTaskAsync(postForeignUserSentinel.Uid, UpdateTimeout);
        await matchingClientTwo.WaitForTaskAsync(postForeignUserSentinel.Uid, UpdateTimeout);
        await differentClient.WaitForTaskAsync(postForeignUserSentinel.Uid, UpdateTimeout);

        Assert.False(matchingClientOne.HasReceived(foreignSentinel.Uid));
        Assert.False(matchingClientTwo.HasReceived(foreignSentinel.Uid));
        Assert.False(differentClient.HasReceived(foreignSentinel.Uid));
        Assert.Equal(
            [target.Uid, userSentinel.Uid, postForeignUserSentinel.Uid],
            differentClient.ArrivalOrder);
    }

    [Fact]
    public async Task RecurrencesCreate_NotifiesConnectedUserThroughTaskHub()
    {
        await using var user = await CreateUserClientAsync();
        await using var client = await TestSignalRClient.CreateAsync(
            user,
            "recurrence-client");
        var today = TestDateService.UtcToday;
        var title = $"Hub recurrence {RecurrencesHelper.CreateUniqueRecurrenceUid()}";
        var recurrence = RecurrencesHelper.CreateRecurrence(
            title,
            today,
            today,
            everyNthDay: 1);

        using var seedResponse = await RecurrencesApi.SaveAsync(
            user.HttpClient,
            [recurrence]);
        Assert.Equal(1, await RecurrencesReader.ReadCountAsync(seedResponse));

        using var createResponse = await RecurrencesApi.CreateTasksAsync(user.HttpClient);
        var createdCount = await RecurrencesReader.ReadCountAsync(createResponse);
        Assert.Equal(1, createdCount);

        var generatedTask = await client.WaitForTaskAsync(
            task => task.Title == title,
            UpdateTimeout);
        Assert.Equal(today, generatedTask.Date);
    }

    private static async Task SaveTaskAsync(
        TestUserClient user,
        TaskDto task,
        string? clientId = null)
    {
        using var response = await TasksApi.SaveAsync(
            user.HttpClient,
            [task],
            clientId);
        Assert.Single(await TasksReader.ReadAsync(response));
    }

    private static TaskDto CreateTask(string title)
    {
        return new TaskDto
        {
            Uid = TasksHelper.CreateUniqueTaskUid(),
            Title = title,
            Date = TestDateService.UtcToday,
            Type = TaskTypeDto.Simple,
        };
    }
}
