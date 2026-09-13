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

public sealed class TelegramIntegrationTests : IntegrationTestBase
{
    private const string Bot = "integration-tests";
    private static readonly TimeSpan MessageTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Start_AnonymousIsRejected_AndRegisteredChatReceivesConfirmation()
    {
        using var anonymousClient = await CreateClientAsync();

        using var anonymousResponse = await TelegramApi.StartAsync(anonymousClient);

        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        var botMessages = await GetBotMessagesAsync();
        await using var user = await CreateUserClientAsync();
        var chatId = TelegramHelper.CreateUniqueChatId();
        using var startResponse = await TelegramApi.StartAsync(user.HttpClient);
        var startKey = await TelegramReader.ReadStartKeyAsync(startResponse);

        using var commandResponse = await TelegramApi.SendCommandAsync(
            anonymousClient,
            chatId,
            $"/start {startKey}",
            Bot);
        commandResponse.EnsureSuccessStatusCode();

        var message = await botMessages.ReadAsync(chatId, MessageTimeout);
        Assert.Equal("Registered", message);
    }

    [Fact]
    public async Task CreateTaskCommand_PersistsTaskForOwner_AndDoesNotExposeItToAnotherUser()
    {
        var botMessages = await GetBotMessagesAsync();
        using var anonymousClient = await CreateClientAsync();
        await using var owner = await CreateUserClientAsync();
        await using var foreignUser = await CreateUserClientAsync();
        var chatId = await RegisterChatAsync(owner, anonymousClient, botMessages);
        var title = $"Telegram task {Guid.NewGuid():N}";

        using var commandResponse = await TelegramApi.SendCommandAsync(
            anonymousClient,
            chatId,
            title,
            Bot);
        commandResponse.EnsureSuccessStatusCode();

        var message = await botMessages.ReadAsync(chatId, MessageTimeout);
        Assert.Equal("Task created", message);

        using var ownerTasksResponse = await TasksApi.LoadAsync(
            owner.HttpClient,
            TestDateService.UtcToday);
        var ownerTasks = await TasksReader.ReadAsync(ownerTasksResponse);
        Assert.Contains(ownerTasks, task => task.Title == title);

        using var foreignTasksResponse = await TasksApi.LoadAsync(
            foreignUser.HttpClient,
            TestDateService.UtcToday);
        var foreignTasks = await TasksReader.ReadAsync(foreignTasksResponse);
        Assert.DoesNotContain(foreignTasks, task => task.Title == title);
    }

    [Fact]
    public async Task BotCommands_WithTasksWithoutTasksAndUnknownCommand_RecordExactMessagesPerChat()
    {
        var botMessages = await GetBotMessagesAsync();
        using var anonymousClient = await CreateClientAsync();
        await using var taskUser = await CreateUserClientAsync();
        await using var emptyUser = await CreateUserClientAsync();
        var taskChatId = await RegisterChatAsync(taskUser, anonymousClient, botMessages);
        var emptyChatId = await RegisterChatAsync(emptyUser, anonymousClient, botMessages);
        var today = TestDateService.UtcToday;
        var taskTitle = $"Telegram todo {Guid.NewGuid():N}";

        using var saveResponse = await TasksApi.SaveAsync(
            taskUser.HttpClient,
            [
                new TaskDto
                {
                    Uid = TasksHelper.CreateUniqueTaskUid(),
                    Date = today,
                    Time = 495,
                    Title = taskTitle,
                    Type = TaskTypeDto.Simple,
                },
            ]);
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        _ = await TasksReader.ReadAsync(saveResponse);

        using var todoResponse = await TelegramApi.SendCommandAsync(
            anonymousClient,
            taskChatId,
            "/todo",
            Bot);
        todoResponse.EnsureSuccessStatusCode();
        Assert.Equal(
            $"08:15 {taskTitle}",
            await botMessages.ReadAsync(taskChatId, MessageTimeout));

        using var emptyTodoResponse = await TelegramApi.SendCommandAsync(
            anonymousClient,
            emptyChatId,
            "/todo",
            Bot);
        emptyTodoResponse.EnsureSuccessStatusCode();
        Assert.Equal(
            "No tasks",
            await botMessages.ReadAsync(emptyChatId, MessageTimeout));

        using var unknownResponse = await TelegramApi.SendCommandAsync(
            anonymousClient,
            emptyChatId,
            "/unknown",
            Bot);
        unknownResponse.EnsureSuccessStatusCode();
        Assert.Equal(
            "Unknown command",
            await botMessages.ReadAsync(emptyChatId, MessageTimeout));
    }

    private static async Task<int> RegisterChatAsync(
        TestUserClient user,
        HttpClient anonymousClient,
        TestBotSendMessageService botMessages)
    {
        var chatId = TelegramHelper.CreateUniqueChatId();
        using var startResponse = await TelegramApi.StartAsync(user.HttpClient);
        var startKey = await TelegramReader.ReadStartKeyAsync(startResponse);
        using var commandResponse = await TelegramApi.SendCommandAsync(
            anonymousClient,
            chatId,
            $"/start {startKey}",
            Bot);
        commandResponse.EnsureSuccessStatusCode();

        var message = await botMessages.ReadAsync(chatId, MessageTimeout);
        Assert.Equal("Registered", message);
        return chatId;
    }
}
