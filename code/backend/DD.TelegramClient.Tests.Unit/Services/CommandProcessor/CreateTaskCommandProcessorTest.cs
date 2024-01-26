using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Implementation.CommandProcessor;
using DD.TelegramClient.Domain.Infrastructure;
using DD.TelegramClient.Domain.Infrastructure.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.CommandProcessor;

public class CreateTaskCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var task = new TaskDto
        {
            Title = "Task",
        };
        var tasks = new TaskDto[0];
        var (telegramMock, taskServiceMock, sendMessageMock) = SetupMocks(task, tasks, 100);

        var service = new CreateTaskCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            taskServiceMock.Object,
            null);

        await service.ProcessAsync(new CreateTaskCommand(task)
        {
            UserChatId = 100
        });


        sendMessageMock.Verify(x => x.SendTextAsync(100, "Task created"));
        taskServiceMock.Verify(x => x.SaveTasksAsync(
            It.Is<ICollection<TaskDto>>(y => y.Any(e =>
                e.Title == "Task" &&
                e.Uid != null
            )),
            "userid"
        ));
    }

    private (Mock<ITelegramService>, Mock<ITaskServiceApp>, Mock<IBotSendMessageService>)
        SetupMocks(TaskDto task, TaskDto[] tasks, int chatId)
    {
        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserId(chatId))
            .Returns(Task.FromResult("userid"));

        var taskServiceMock = new Mock<ITaskServiceApp>();
        taskServiceMock.Setup(x => x.SaveTasksAsync(It.Is<ICollection<TaskDto>>(v => v.Contains(task)), "userid"))
            .Returns(Task.FromResult((IEnumerable<TaskDto>) tasks));

        var sendMessageMock = new Mock<IBotSendMessageService>();

        return (telegramMock, taskServiceMock, sendMessageMock);
    }
}
