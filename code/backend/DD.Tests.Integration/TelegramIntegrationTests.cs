using System.Net;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration;

public sealed class TelegramIntegrationTests : IntegrationTestBase
{
    private static readonly TimeSpan MessageTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Start_AnonymousIsRejected_AndRegisteredChatReceivesConfirmation()
    {
        using var anonymousClient = await CreateClientAsync();

        using var anonymousResponse = await anonymousClient.PostAsync(
            new Uri("api/tlgm/start?timezoneOffset=0", UriKind.Relative),
            content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        var recorder = await GetTelegramMessagesAsync();
        await using var user = await CreateUserClientAsync();
        var chatId = TelegramHelper.CreateUniqueChatId();
        var startKey = await TelegramHelper.CreateStartKeyAsync(user);

        await TelegramHelper.SendCommandAsync(anonymousClient, chatId, $"/start {startKey}");

        var message = await TelegramHelper.ReadMessageAsync(recorder, chatId, MessageTimeout);
        Assert.Equal("Registered", message);
    }

    [Fact]
    public async Task CreateTaskCommand_PersistsTaskForOwner_AndDoesNotExposeItToAnotherUser()
    {
        var recorder = await GetTelegramMessagesAsync();
        using var anonymousClient = await CreateClientAsync();
        await using var owner = await CreateUserClientAsync();
        await using var foreignUser = await CreateUserClientAsync();
        var chatId = await RegisterChatAsync(owner, anonymousClient, recorder);
        var title = $"Telegram task {Guid.NewGuid():N}";

        await TelegramHelper.SendCommandAsync(anonymousClient, chatId, title);

        var message = await TelegramHelper.ReadMessageAsync(recorder, chatId, MessageTimeout);
        Assert.Equal("Task created", message);

        using var ownerTasksResponse = await owner.HttpClient.GetAsync(
            CreateTasksUri(IntegrationTestClock.UtcToday));
        var ownerTasks = await ReadTasksAsync(ownerTasksResponse);
        Assert.Contains(ownerTasks, task => task.Title == title);

        using var foreignTasksResponse = await foreignUser.HttpClient.GetAsync(
            CreateTasksUri(IntegrationTestClock.UtcToday));
        var foreignTasks = await ReadTasksAsync(foreignTasksResponse);
        Assert.DoesNotContain(foreignTasks, task => task.Title == title);
    }

    [Fact]
    public async Task BotCommands_WithTasksWithoutTasksAndUnknownCommand_RecordExactMessagesPerChat()
    {
        var recorder = await GetTelegramMessagesAsync();
        using var anonymousClient = await CreateClientAsync();
        await using var taskUser = await CreateUserClientAsync();
        await using var emptyUser = await CreateUserClientAsync();
        var taskChatId = await RegisterChatAsync(taskUser, anonymousClient, recorder);
        var emptyChatId = await RegisterChatAsync(emptyUser, anonymousClient, recorder);
        var today = IntegrationTestClock.UtcToday;
        var taskTitle = $"Telegram todo {Guid.NewGuid():N}";

        using var saveResponse = await taskUser.HttpClient.PostAsJsonAsync(
            "api/task/tasks",
            new[]
            {
                new TaskDto
                {
                    Uid = CreateUniqueTaskUid(),
                    Date = today,
                    Time = 495,
                    Title = taskTitle,
                    Type = TaskTypeDto.Simple,
                },
            });
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        _ = await ReadTasksAsync(saveResponse);

        await TelegramHelper.SendCommandAsync(anonymousClient, taskChatId, "/todo");
        Assert.Equal(
            $"08:15 {taskTitle}",
            await TelegramHelper.ReadMessageAsync(recorder, taskChatId, MessageTimeout));

        await TelegramHelper.SendCommandAsync(anonymousClient, emptyChatId, "/todo");
        Assert.Equal(
            "No tasks",
            await TelegramHelper.ReadMessageAsync(recorder, emptyChatId, MessageTimeout));

        await TelegramHelper.SendCommandAsync(anonymousClient, emptyChatId, "/unknown");
        Assert.Equal(
            "Unknown command",
            await TelegramHelper.ReadMessageAsync(recorder, emptyChatId, MessageTimeout));
    }

    private static async Task<int> RegisterChatAsync(
        TestUserClient user,
        HttpClient anonymousClient,
        RecordingBotSendMessageService recorder)
    {
        var chatId = TelegramHelper.CreateUniqueChatId();
        var startKey = await TelegramHelper.CreateStartKeyAsync(user);
        await TelegramHelper.SendCommandAsync(anonymousClient, chatId, $"/start {startKey}");

        var message = await TelegramHelper.ReadMessageAsync(recorder, chatId, MessageTimeout);
        Assert.Equal("Registered", message);
        return chatId;
    }
}
