using DD.TelegramClient.Domain.Models.Commands;
using DD.TelegramClient.Domain.Services;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TelegramClient.Services;

public class BotCommandParserServiceTest
{
    [Fact]
    public async Task ParseCommand_CreateTask()
    {
        var telegramMock = new Mock<ITelegramService>();
        var dateServiceMock = new Mock<IDateService>();
        var service = new BotCommandParserService(telegramMock.Object, dateServiceMock.Object);

        var result = await service.ParseCommand("Some task", 100500);

        Assert.NotNull(result);
        Assert.IsType<CreateTaskCommand>(result);
        Assert.Equal("Some task", ((CreateTaskCommand)result).Text);
    }

    [Fact]
    public async Task ProcessMessage_ShowTodo()
    {
        var dateServiceMock = new Mock<IDateService>();
        dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2010, 10, 10));
        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
        var service = new BotCommandParserService(telegramMock.Object, dateServiceMock.Object);

        var result = await service.ParseCommand("/todo", 100500);

        Assert.NotNull(result);
        Assert.IsType<ShowTodoCommand>(result);
        Assert.Equal(new DateTime(2010, 10, 10), ((ShowTodoCommand)result).From);
        Assert.Equal(new DateTime(2010, 10, 11), ((ShowTodoCommand)result).To);
    }

    [Fact]
    public async Task ProcessMessage_Start()
    {
        var dateServiceMock = new Mock<IDateService>();
        var telegramServiceMock = new Mock<ITelegramService>();
        var service = new BotCommandParserService(telegramServiceMock.Object, dateServiceMock.Object);

        var result = await service.ParseCommand("/start SomeChatKey", 0);

        Assert.NotNull(result);
        Assert.IsType<StartCommand>(result);
        Assert.Equal("SomeChatKey", ((StartCommand)result).UserChatKey);
    }
}
