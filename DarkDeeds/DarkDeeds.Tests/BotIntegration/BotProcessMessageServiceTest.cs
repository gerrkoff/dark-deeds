using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Implementation;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration
{
    public partial class BotProcessMessageServiceTest : BaseTest
    {
        [Fact]
        public void BotProcessMessageServiceTest_SendUnknownCommand()
        {
            var commandParserMock = new Mock<IBotCommandParserService>();
            commandParserMock.Setup(x => x.ParseCommand(It.IsAny<string>())).Returns<BotCommand>(null);
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object, commandParserMock.Object, null, null);

            service.ProcessMessage(new UpdateDto {Message = new MessageDto {Text = ""}});
            
            sendMsgMock.Verify(x => x.SendUnknownCommand());
            sendMsgMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public void BotProcessMessageServiceTest_RunShowTodoCommand()
        {
            var command = new ShowTodoCommand("");
            var commandParserMock = new Mock<IBotCommandParserService>();
            commandParserMock.Setup(x => x.ParseCommand(It.IsAny<string>())).Returns(command);
            var commandMock = new Mock<IBotProcessShowTodoService>();
            var service = new BotProcessMessageService(null, commandParserMock.Object, commandMock.Object, null);

            service.ProcessMessage(new UpdateDto {Message = new MessageDto {Text = ""}});
            
            commandMock.Verify(x => x.Process(command));
            commandMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public void BotProcessMessageServiceTest_RunCreateTaskCommand()
        {
            var command = new CreateTaskCommand(null);
            var commandParserMock = new Mock<IBotCommandParserService>();
            commandParserMock.Setup(x => x.ParseCommand(It.IsAny<string>())).Returns(command);
            var commandMock = new Mock<IBotProcessCreateTaskService>();
            var service = new BotProcessMessageService(null, commandParserMock.Object, null, commandMock.Object);

            service.ProcessMessage(new UpdateDto {Message = new MessageDto {Text = ""}});
            
            commandMock.Verify(x => x.Process(command));
            commandMock.VerifyNoOtherCalls();
        }
    }
}