using DD.ServiceTask.Domain.Exceptions;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using DD.TelegramClient.Domain.Services;
using DD.TelegramClient.Domain.Services.CommandProcessor;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TelegramClient.Services.CommandProcessor;

public class CreateTaskCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var task = new TaskDto
        {
            Title = "Task",
        };
        TaskDto[] tasks = [];
        var (telegramMock, taskServiceMock, sendMessageMock, loggerMock) =
            SetupMocks(task, tasks, 100);

        var service = new CreateTaskCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            taskServiceMock.Object,
            loggerMock.Object);

        await service.ProcessAsync(new CreateTaskCommand("Some task")
        {
            UserChatId = 100,
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "Task created"));
        taskServiceMock.Verify(x => x.SaveTasksAsync(
            It.Is<ICollection<TaskDto>>(y => y.Any(e => e.Title == "Task")),
            "userid",
            It.IsAny<string?>()));
    }

    [Fact]
    public async Task ProcessAsync_WhenParseThrows_SendsFailedAndDoesNotSave()
    {
        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserId(100)).Returns(Task.FromResult("userid"));

        var taskServiceMock = new Mock<ITaskServiceApp>();
        taskServiceMock.Setup(x => x.ParseTasks(It.IsAny<string>()))
            .Throws(ServiceException.InvalidDateRange(31));

        var sendMessageMock = new Mock<IBotSendMessageService>();
        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();

        var service = new CreateTaskCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            taskServiceMock.Object,
            loggerMock.Object);

        await service.ProcessAsync(new CreateTaskCommand("0901-1002 venice")
        {
            UserChatId = 100,
        });

        sendMessageMock.Verify(x => x.SendFailedAsync(100));
        taskServiceMock.Verify(
            x => x.SaveTasksAsync(It.IsAny<ICollection<TaskDto>>(), It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
    }

    private static (
        Mock<ITelegramService> TelegramServiceMock,
        Mock<ITaskServiceApp> TaskServiceAppMock,
        Mock<IBotSendMessageService> BotSendMessageServiceMock,
        Mock<ILogger<BaseCommandProcessor<BotCommand>>> LoggerMock)
        SetupMocks(TaskDto task, TaskDto[] tasks, int chatId)
    {
        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserId(chatId))
            .Returns(Task.FromResult("userid"));

        var taskServiceMock = new Mock<ITaskServiceApp>();
        taskServiceMock.Setup(x => x.ParseTasks("Some task"))
            .Returns(Task.FromResult<IReadOnlyList<TaskDto>>([task]));
        taskServiceMock.Setup(x => x.SaveTasksAsync(It.Is<ICollection<TaskDto>>(v => v.Contains(task)), "userid", It.IsAny<string?>()))
            .Returns(Task.FromResult((IEnumerable<TaskDto>)tasks));

        var sendMessageMock = new Mock<IBotSendMessageService>();

        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();

        return (telegramMock, taskServiceMock, sendMessageMock, loggerMock);
    }
}
