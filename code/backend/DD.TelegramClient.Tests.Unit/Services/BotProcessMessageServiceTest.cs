using System.Diagnostics.CodeAnalysis;
using DD.TelegramClient.Domain.Dto;
using DD.TelegramClient.Domain.Infrastructure.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using DD.TelegramClient.Domain.Services;
using DD.TelegramClient.Domain.Services.CommandProcessor;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class BotProcessMessageServiceTest
{
    private static UpdateDto UpdateEmpty => new()
    {
        Message = new MessageDto
        {
            Text = string.Empty,
            Chat = new ChatDto
            {
                Id = 1,
            },
        },
    };

    [Fact]
    public async Task BotProcessMessageServiceTest_SendUnknownCommand()
    {
        var showTodoCommandProcessorMock = new Mock<IShowTodoCommandProcessor>();
        var createTaskCommandProcessorMock = new Mock<ICreateTaskCommandProcessor>();
        var startCommandProcessorMock = new Mock<IStartCommandProcessor>();
        var commandParserMock = CreateCommandParserMock(null);
        var sendMsgMock = new Mock<IBotSendMessageService>();
        var service = new BotProcessMessageService(
            sendMsgMock.Object,
            commandParserMock.Object,
            showTodoCommandProcessorMock.Object,
            createTaskCommandProcessorMock.Object,
            startCommandProcessorMock.Object);

        await service.ProcessMessageAsync(UpdateEmpty);

        sendMsgMock.Verify(x => x.SendUnknownCommandAsync(It.IsAny<int>()));
        sendMsgMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task BotProcessMessageServiceTest_RunShowTodoCommand()
    {
        var command = new ShowTodoCommand(string.Empty, new DateTime(), 0);
        var commandParserMock = CreateCommandParserMock(command);
        var commandMock = new Mock<IShowTodoCommandProcessor>();
        var botSendMessageServiceMock = new Mock<IBotSendMessageService>();
        var createTaskCommandProcessorMock = new Mock<ICreateTaskCommandProcessor>();
        var startCommandProcessorMock = new Mock<IStartCommandProcessor>();
        var service = new BotProcessMessageService(
            botSendMessageServiceMock.Object,
            commandParserMock.Object,
            commandMock.Object,
            createTaskCommandProcessorMock.Object,
            startCommandProcessorMock.Object);

        await service.ProcessMessageAsync(UpdateEmpty);

        commandMock.Verify(x => x.ProcessAsync(command));
        commandMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task BotProcessMessageServiceTest_RunCreateTaskCommand()
    {
        var task = new TaskDto();
        var command = new CreateTaskCommand(task);
        var commandParserMock = CreateCommandParserMock(command);
        var commandMock = new Mock<ICreateTaskCommandProcessor>();
        var botSendMessageServiceMock = new Mock<IBotSendMessageService>();
        var startCommandProcessorMock = new Mock<IStartCommandProcessor>();
        var showTodoCommandProcessorMock = new Mock<IShowTodoCommandProcessor>();
        var service = new BotProcessMessageService(
            botSendMessageServiceMock.Object,
            commandParserMock.Object,
            showTodoCommandProcessorMock.Object,
            commandMock.Object,
            startCommandProcessorMock.Object);

        await service.ProcessMessageAsync(UpdateEmpty);

        commandMock.Verify(x => x.ProcessAsync(command));
        commandMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task BotProcessMessageServiceTest_RunStartCommand()
    {
        var command = new StartCommand(string.Empty);
        var commandParserMock = CreateCommandParserMock(command);
        var commandMock = new Mock<IStartCommandProcessor>();
        var botSendMessageServiceMock = new Mock<IBotSendMessageService>();
        var showTodoCommandProcessorMock = new Mock<IShowTodoCommandProcessor>();
        var createTaskCommandProcessorMock = new Mock<ICreateTaskCommandProcessor>();
        var service = new BotProcessMessageService(
            botSendMessageServiceMock.Object,
            commandParserMock.Object,
            showTodoCommandProcessorMock.Object,
            createTaskCommandProcessorMock.Object,
            commandMock.Object);

        await service.ProcessMessageAsync(UpdateEmpty);

        commandMock.Verify(x => x.ProcessAsync(command));
        commandMock.VerifyNoOtherCalls();
    }

    private static Mock<IBotCommandParserService> CreateCommandParserMock(BotCommand? command)
    {
        var commandParserMock = new Mock<IBotCommandParserService>();
        commandParserMock.Setup(x => x.ParseCommand(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(() => Task.FromResult(command));
        return commandParserMock;
    }
}
