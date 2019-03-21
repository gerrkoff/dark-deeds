using DarkDeeds.BotIntegration.Implementation;
using DarkDeeds.BotIntegration.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration
{
    public partial class BotProcessMessageServiceTest : BaseTest
    {
        [Fact]
        public void ProcessMessage_ParseCmdWithArgs()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo qwerty", out string args);

            Assert.True(result);
            Assert.Equal("qwerty", args);
        }
        
        [Fact]
        public void ProcessMessage_ParseCmdWithoutArgs()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo", out string args);

            Assert.True(result);
            Assert.Equal(string.Empty, args);
        }
        
        [Fact]
        public void ProcessMessage_ParseCmdWithEmptyArgs()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo ", out string args);

            Assert.True(result);
            Assert.Equal(string.Empty, args);
        }
        
        [Fact]
        public void ProcessMessage_ReturnFalseIfWronCmd()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo1 qwerty", out string args);

            Assert.False(result);
            Assert.Equal(string.Empty, args);
        }
    }
}