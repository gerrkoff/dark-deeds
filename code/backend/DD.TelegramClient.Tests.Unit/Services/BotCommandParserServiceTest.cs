using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Infrastructure;
using DD.TelegramClient.Domain.Infrastructure.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services;

public class BotCommandParserServiceTest
{
    [Fact]
    public async Task ParseCommand_CreateTask()
    {
        var task = new TaskDto();
        var telegramMock = new Mock<ITelegramService>();
        var taskParserMock = new Mock<ITaskServiceApp>();
        taskParserMock.Setup(x => x.ParseTask("Some task")).Returns(Task.FromResult(task));
        var dateServiceMock = new Mock<IDateService>();
        var service = new BotCommandParserService(telegramMock.Object, dateServiceMock.Object, taskParserMock.Object);

        var result = await service.ParseCommand("Some task", 100500);

        Assert.NotNull(result);
        Assert.IsType<CreateTaskCommand>(result);
        Assert.Same(task, ((CreateTaskCommand) result!).Task);
    }

    [Fact]
    public async Task ProcessMessage_ShowTodo()
    {
        var dateServiceMock = new Mock<IDateService>();
        dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2010, 10, 10));
        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
        var taskServiceAppMock = new Mock<ITaskServiceApp>();
        var service = new BotCommandParserService(telegramMock.Object, dateServiceMock.Object, taskServiceAppMock.Object);

        var result = await service.ParseCommand("/todo", 100500);

        Assert.NotNull(result);
        Assert.IsType<ShowTodoCommand>(result);
        Assert.Equal(new DateTime(2010, 10, 10), ((ShowTodoCommand) result!).From);
        Assert.Equal(new DateTime(2010, 10, 11), ((ShowTodoCommand) result).To);
    }

    [Fact]
    public async Task ProcessMessage_Start()
    {
        var dateServiceMock = new Mock<IDateService>();
        var telegramServiceMock = new Mock<ITelegramService>();
        var taskServiceAppMock = new Mock<ITaskServiceApp>();
        var service = new BotCommandParserService(telegramServiceMock.Object, dateServiceMock.Object, taskServiceAppMock.Object);

        var result = await service.ParseCommand("/start SomeChatKey", 0);

        Assert.NotNull(result);
        Assert.IsType<StartCommand>(result);
        Assert.Equal("SomeChatKey", ((StartCommand) result!).UserChatKey);
    }
}
