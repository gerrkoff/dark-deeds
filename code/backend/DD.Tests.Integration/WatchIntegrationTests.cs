using System.Net;
using System.Net.Http.Json;
using DD.Clients.Details.MobileClient.Data;
using DD.MobileClient.Domain.Dto;
using DD.MobileClient.Domain.Entities;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;
using static DD.Tests.Integration.Helpers.MobileHelper;

namespace DD.Tests.Integration;

public sealed class WatchIntegrationTests : IntegrationTestBase
{
    private static readonly TimeSpan NotificationTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PayloadTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Watch_ProvisionedUser_ReturnsFilteredWidgetAndAppPayloads()
    {
        await using var user = await CreateUserClientAsync();
        var mobileKey = await SeedMobileUserAsync(user);
        var today = DateTime.UtcNow.Date;
        var tasks = CreateWatchTasks(today, today.AddDays(1));

        using var saveResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            tasks);
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        Assert.Equal(tasks.Length, (await ReadTasksAsync(saveResponse)).Length);

        using var anonymousClient = await CreateClientAsync();
        var widget = await ReadWidgetAsync(anonymousClient, mobileKey);
        Assert.Equal("\ud83d\udccc 2 remaining", widget.Header);
        Assert.Equal("Simple task", widget.Main);
        Assert.Equal("Routine task", widget.Support);

        var app = await ReadAppAsync(anonymousClient, mobileKey);
        Assert.Equal("\ud83d\udccc 2 remaining", app.Header);
        AssertAppItems(app, "Simple task");
    }

    [Fact]
    public async Task Watch_TaskUpdateInvalidatesWidgetAndAppCaches()
    {
        await using var user = await CreateUserClientAsync();
        var mobileKey = await SeedMobileUserAsync(user);
        var today = DateTime.UtcNow.Date;
        var tasks = CreateWatchTasks(today, today.AddDays(1));

        using var setupResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            tasks);
        var savedTasks = await ReadTasksAsync(setupResponse);

        await using var sentinelUser = await CreateUserClientAsync();
        using var sentinelHandler = await CreateSignalRHandlerAsync();
        await using var sentinelCollector = await SignalRUpdateCollector.ConnectAsync(
            sentinelHandler,
            sentinelUser.Token,
            "mobile-cache-sentinel");

        var sentinel = new TaskDto
        {
            Uid = CreateUniqueTaskUid(),
            Date = today,
            Title = $"Mobile sentinel {Guid.NewGuid():N}",
            Type = TaskTypeDto.Simple,
        };
        using var sentinelResponse = await sentinelUser.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[] { sentinel });
        _ = await ReadTasksAsync(sentinelResponse);
        await sentinelCollector.WaitForTaskAsync(sentinel.Uid, NotificationTimeout);

        using var anonymousClient = await CreateClientAsync();
        var initialWidget = await ReadWidgetAsync(anonymousClient, mobileKey);
        var initialApp = await ReadAppAsync(anonymousClient, mobileKey);
        Assert.Equal("Simple task", initialWidget.Main);
        Assert.Equal("Simple task", initialApp.Items[1].Item);

        var updatedTasks = savedTasks.Where(task => task.Title == "Simple task").ToArray();
        Assert.Equal(2, updatedTasks.Length);
        foreach (var updatedTask in updatedTasks)
            updatedTask.Title = "Updated simple task";

        using var updateResponse = await user.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            updatedTasks);
        var savedUpdates = await ReadTasksAsync(updateResponse);
        Assert.Equal(2, savedUpdates.Length);
        Assert.All(savedUpdates, savedUpdate => Assert.Equal(2, savedUpdate.Version));

        var updatedWidget = await PollUntilAsync(
            () => ReadWidgetAsync(anonymousClient, mobileKey),
            payload => payload.Main == "Updated simple task",
            PayloadTimeout);
        Assert.Equal(initialWidget.Header, updatedWidget.Header);
        Assert.Equal("Routine task", updatedWidget.Support);

        var updatedApp = await PollUntilAsync(
            () => ReadAppAsync(anonymousClient, mobileKey),
            payload => payload.Items.Any(item => item.Item == "Updated simple task"),
            PayloadTimeout);
        Assert.Equal(initialApp.Header, updatedApp.Header);
        AssertAppItems(updatedApp, "Updated simple task");
    }

    [Fact]
    public async Task Watch_UnknownMobileKey_ReturnsUnexpectedErrorProblemDetails()
    {
        using var client = await CreateClientAsync();
        var mobileKey = CreateUniqueMobileKey();

        using var response = await client.GetAsync(CreateWidgetUri(mobileKey));

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("An unexpected error occurred", problemDetails.Title);
    }

    private static void AssertAppItems(WatchAppStatusDto app, string simpleTaskTitle)
    {
        Assert.Collection(
            app.Items,
            item =>
            {
                Assert.Equal("Routine task", item.Item);
                Assert.True(item.IsSupport);
            },
            item =>
            {
                Assert.Equal(simpleTaskTitle, item.Item);
                Assert.False(item.IsSupport);
            },
            item =>
            {
                Assert.Equal("10:15 Timed task", item.Item);
                Assert.False(item.IsSupport);
            });
    }

    private static async Task<string> SeedMobileUserAsync(TestUserClient user)
    {
        var mobileKey = CreateUniqueMobileKey();
        var userId = GetUserId(user.Token);

        await ExecuteScopedAsync(async services =>
        {
            var repository = services.GetRequiredService<MobileUserRepository>();
            await repository.UpsertAsync(new MobileUserEntity
            {
                MobileKey = mobileKey,
                UserId = userId,
            });
            return true;
        });

        return mobileKey;
    }

    private static TaskDto[] CreateWatchTasks(params DateTime[] dates)
    {
        return dates.SelectMany(date =>
            new TaskDto[]
            {
                new()
                {
                    Uid = CreateUniqueTaskUid(),
                    Date = date,
                    Order = 1,
                    Title = "Routine task",
                    Type = TaskTypeDto.Routine,
                },
                new()
                {
                    Uid = CreateUniqueTaskUid(),
                    Date = date,
                    Order = 2,
                    Title = "Simple task",
                    Type = TaskTypeDto.Simple,
                },
                new()
                {
                    Uid = CreateUniqueTaskUid(),
                    Date = date,
                    Order = 3,
                    Title = "Additional task",
                    Type = TaskTypeDto.Additional,
                },
                new()
                {
                    Uid = CreateUniqueTaskUid(),
                    Date = date,
                    Order = 4,
                    Title = "Completed task",
                    Completed = true,
                    Type = TaskTypeDto.Simple,
                },
                new()
                {
                    Uid = CreateUniqueTaskUid(),
                    Date = date,
                    Order = 5,
                    Time = 615,
                    Title = "Timed task",
                    Type = TaskTypeDto.Simple,
                },
            }).ToArray();
    }
}
