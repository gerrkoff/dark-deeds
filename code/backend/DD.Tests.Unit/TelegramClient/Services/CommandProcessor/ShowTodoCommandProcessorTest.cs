using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using DD.TelegramClient.Domain.Services;
using DD.TelegramClient.Domain.Services.CommandProcessor;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TelegramClient.Services.CommandProcessor;

public class ShowTodoCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var (telegramMock, taskServiceMock, sendMessageMock, taskPrinterMock, loggerMock) =
            SetupMocks([new TaskDto()], "tasks", 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            loggerMock.Object,
            taskServiceMock.Object,
            taskPrinterMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100,
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "tasks"));
    }

    [Fact]
    public async Task ProcessAsync_NoTasks()
    {
        var (telegramMock, taskServiceMock, sendMessageMock, taskPrinterMock, loggerMock) =
            SetupMocks([], string.Empty, 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            loggerMock.Object,
            taskServiceMock.Object,
            taskPrinterMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100,
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "No tasks"));
    }

    private static (Mock<ITelegramService> TelegramServiceMock, Mock<ITaskServiceApp> TaskServiceAppMock, Mock<IBotSendMessageService> BotSendMessageServiceMock, Mock<ITaskPrinter> TaskPrinterMock, Mock<ILogger<BaseCommandProcessor<BotCommand>>> LoggerMock)
        SetupMocks(ICollection<TaskDto> tasks, string taskAsString, int chatId)
    {
        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserId(chatId))
            .Returns(Task.FromResult("userid"));

        var taskPrinterMock = new Mock<ITaskPrinter>();
        taskPrinterMock.Setup(x => x.PrintWithSymbolCodes(It.IsAny<TaskDto>()))
            .Returns(taskAsString);

        var taskServiceMock = new Mock<ITaskServiceApp>();
        taskServiceMock.Setup(x => x.LoadTasksByDateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "userid"))
            .ReturnsAsync(tasks);

        var sendMessageMock = new Mock<IBotSendMessageService>();

        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();

        return (telegramMock, taskServiceMock, sendMessageMock, taskPrinterMock, loggerMock);
    }
}
