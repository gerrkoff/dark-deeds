using System;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Implementation;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration
{
    public class BotProcessMessageServiceTest : BaseTest
    {
        private UpdateDto UpdateEmpty => new UpdateDto
        {
            Message = new MessageDto
            {
                Text = string.Empty,
                Chat = new ChatDto
                {
                    Id = 1
                }
            }
        };

        private Mock<IBotCommandParserService> createCommandParserMock(BotCommand command)
        {
            var commandParserMock = new Mock<IBotCommandParserService>();
            commandParserMock.Setup(x => x.ParseCommand(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() => Task.FromResult<BotCommand>(command));
            return commandParserMock;
        }
        
        [Fact]
        public async Task BotProcessMessageServiceTest_SendUnknownCommand()
        {
            var commandParserMock = createCommandParserMock(null);
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(
                sendMsgMock.Object, 
                commandParserMock.Object,
                null, 
                null, 
                null);

            await service.ProcessMessageAsync(UpdateEmpty);
            
            sendMsgMock.Verify(x => x.SendUnknownCommandAsync(It.IsAny<int>()));
            sendMsgMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task BotProcessMessageServiceTest_RunShowTodoCommand()
        {
            var command = new ShowTodoCommand("", new DateTime(), 0);
            var commandParserMock = createCommandParserMock(command);
            var commandMock = new Mock<IShowTodoCommandProcessor>();
            var service = new BotProcessMessageService(
                null, 
                commandParserMock.Object,
                commandMock.Object,
                null,
                null);

            await service.ProcessMessageAsync(UpdateEmpty);
            
            commandMock.Verify(x => x.ProcessAsync(command));
            commandMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task BotProcessMessageServiceTest_RunCreateTaskCommand()
        {
            var command = new CreateTaskCommand(null);
            var commandParserMock = createCommandParserMock(command);
            var commandMock = new Mock<ICreateTaskCommandProcessor>();
            var service = new BotProcessMessageService(
                null, 
                commandParserMock.Object, 
                null, 
                commandMock.Object,
                null);

            await service.ProcessMessageAsync(UpdateEmpty);
            
            commandMock.Verify(x => x.ProcessAsync(command));
            commandMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task BotProcessMessageServiceTest_RunStartCommand()
        {
            var command = new StartCommand(string.Empty);
            var commandParserMock = createCommandParserMock(command);
            var commandMock = new Mock<IStartCommandProcessor>();
            var service = new BotProcessMessageService(
                null, 
                commandParserMock.Object, 
                null, 
                null,
                commandMock.Object);

            await service.ProcessMessageAsync(UpdateEmpty);
            
            commandMock.Verify(x => x.ProcessAsync(command));
            commandMock.VerifyNoOtherCalls();
        }
    }
}