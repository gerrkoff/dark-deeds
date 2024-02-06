using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Implementation.CommandProcessor;
using DD.TelegramClient.Domain.Infrastructure;
using DD.TelegramClient.Domain.Infrastructure.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.CommandProcessor;

public class ShowTodoCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var (telegramMock, taskServiceMock, sendMessageMock, loggerMock) = SetupMocks(new[]{"tasks"}, 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            loggerMock.Object,
            taskServiceMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "tasks"));
    }

    [Fact]
    public async Task ProcessAsync_NoTasks()
    {
        var (telegramMock, taskServiceMock, sendMessageMock, loggerMock) = SetupMocks(new string[0], 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            loggerMock.Object,
            taskServiceMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "No tasks"));
    }

    private (Mock<ITelegramService>, Mock<ITaskServiceApp>, Mock<IBotSendMessageService>, Mock<ILogger<BaseCommandProcessor<BotCommand>>>)
        SetupMocks(ICollection<string> tasksAsString, int chatId)
    {
        var tasks = new TaskDto[0];

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
