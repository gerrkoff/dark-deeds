using System.Diagnostics.CodeAnalysis;
using DD.TelegramClient.Domain.Infrastructure;
using DD.TelegramClient.Domain.Infrastructure.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using DD.TelegramClient.Domain.Services;
using DD.TelegramClient.Domain.Services.CommandProcessor;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.CommandProcessor;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class ShowTodoCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var (telegramMock, taskServiceMock, sendMessageMock, loggerMock) =
            SetupMocks(new[] { "tasks" }, 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            loggerMock.Object,
            taskServiceMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100,
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "tasks"));
    }

    [Fact]
    public async Task ProcessAsync_NoTasks()
    {
        var (telegramMock, taskServiceMock, sendMessageMock, loggerMock) =
            SetupMocks(Array.Empty<string>(), 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            loggerMock.Object,
            taskServiceMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100,
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "No tasks"));
    }

    private static (
        Mock<ITelegramService> TelegramServiceMock,
        Mock<ITaskServiceApp> TaskServiceAppMock,
        Mock<IBotSendMessageService> BotSendMessageServiceMock,
        Mock<ILogger<BaseCommandProcessor<BotCommand>>> LoggerMock)
        SetupMocks(ICollection<string> tasksAsString, int chatId)
    {
        TaskDto[] tasks = [];

        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserId(chatId))
            .Returns(Task.FromResult("userid"));

        var taskServiceMock = new Mock<ITaskServiceApp>();
        taskServiceMock.Setup(x => x.PrintTasks(tasks))
            .Returns(Task.FromResult(tasksAsString));

        var sendMessageMock = new Mock<IBotSendMessageService>();

        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();

        return (telegramMock, taskServiceMock, sendMessageMock, loggerMock);
    }
}
