using System.Net;
using DD.MobileClient.Domain.Dto;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Clients;
using DD.Tests.Integration.Infrastructure.Readers;
using Xunit;
using static DD.Tests.Integration.Helpers.MobileHelper;
using static DD.Tests.Integration.Helpers.TasksTestData;

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

        using var saveResponse = await TasksApi.SaveAsync(user.HttpClient, tasks);
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        Assert.Equal(tasks.Length, (await TasksReader.ReadAsync(saveResponse)).Length);

        using var anonymousClient = await CreateClientAsync();
        using var widgetResponse = await MobileApi.GetWidgetAsync(
            anonymousClient,
            mobileKey);
        var widget = await MobileReader.ReadWidgetAsync(widgetResponse);
        Assert.Equal("\ud83d\udccc 2 remaining", widget.Header);
        Assert.Equal("Simple task", widget.Main);
        Assert.Equal("Routine task", widget.Support);

        using var appResponse = await MobileApi.GetAppAsync(anonymousClient, mobileKey);
        var app = await MobileReader.ReadAppAsync(appResponse);
        Assert.Equal("\ud83d\udccc 2 remaining", app.Header);
        AssertAppItems(app, "Simple task");
    }

    [Fact]
    public async Task Watch_TaskUpdateInvalidatesWidgetAndAppCaches()
    {
        await using var user = await CreateUserClientAsync();
        var mobileKey = await SeedMobileUserAsync(user);
        var today = DateTime.UtcNow.Date;
        var tasks = CreateWatchTasks(today);

        using var setupResponse = await TasksApi.SaveAsync(user.HttpClient, tasks);
        var savedTasks = await TasksReader.ReadAsync(setupResponse);

        await using var sentinelUser = await CreateUserClientAsync();
        await using var sentinelClient = await TestSignalRClient.CreateAsync(
            sentinelUser,
            "mobile-cache-sentinel");

        var sentinel = new TaskDto
        {
            Uid = CreateUniqueTaskUid(),
            Date = today,
            Title = $"Mobile sentinel {Guid.NewGuid():N}",
            Type = TaskTypeDto.Simple,
        };
        using var sentinelResponse = await TasksApi.SaveAsync(
            sentinelUser.HttpClient,
            [sentinel]);
        _ = await TasksReader.ReadAsync(sentinelResponse);
        await sentinelClient.WaitForTaskAsync(sentinel.Uid, NotificationTimeout);

        using var anonymousClient = await CreateClientAsync();
        using var initialWidgetResponse = await MobileApi.GetWidgetAsync(
            anonymousClient,
            mobileKey);
        var initialWidget = await MobileReader.ReadWidgetAsync(initialWidgetResponse);
        using var initialAppResponse = await MobileApi.GetAppAsync(
            anonymousClient,
            mobileKey);
        var initialApp = await MobileReader.ReadAppAsync(initialAppResponse);
        Assert.Equal("Simple task", initialWidget.Main);
        Assert.Equal("Simple task", initialApp.Items[1].Item);

        var updatedTasks = savedTasks.Where(task => task.Title == "Simple task").ToArray();
        Assert.Single(updatedTasks);
        foreach (var updatedTask in updatedTasks)
            updatedTask.Title = "Updated simple task";

        using var updateResponse = await TasksApi.SaveAsync(user.HttpClient, updatedTasks);
        var savedUpdates = await TasksReader.ReadAsync(updateResponse);
        Assert.Single(savedUpdates);
        Assert.All(savedUpdates, savedUpdate => Assert.Equal(2, savedUpdate.Version));

        var updatedWidget = await PollUntilAsync(
            async () =>
            {
                using var response = await MobileApi.GetWidgetAsync(
                    anonymousClient,
                    mobileKey);
                return await MobileReader.ReadWidgetAsync(response);
            },
            payload => payload.Main == "Updated simple task",
            PayloadTimeout);
        Assert.Equal(initialWidget.Header, updatedWidget.Header);
        Assert.Equal("Routine task", updatedWidget.Support);

        var updatedApp = await PollUntilAsync(
            async () =>
            {
                using var response = await MobileApi.GetAppAsync(
                    anonymousClient,
                    mobileKey);
                return await MobileReader.ReadAppAsync(response);
            },
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

        using var response = await MobileApi.GetWidgetAsync(client, mobileKey);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var problemDetails = await ProblemDetailsReader.ReadAsync(response);
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

        using var response = await MobileApi.CreateUserMappingAsync(
            user.HttpClient,
            userId,
            mobileKey);
        response.EnsureSuccessStatusCode();

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
