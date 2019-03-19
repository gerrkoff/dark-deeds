using DarkDeeds.Models.Bot;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class BotProcessMessageServiceTest : BaseTest
    {
        [Fact]
        public void ProcessMessage_ParseCmdWithArgs()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo qwerty", out string args);

            Assert.Equal(true, result);
            Assert.Equal("qwerty", args);
        }
        
        [Fact]
        public void ProcessMessage_ParseCmdWithoutArgs()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo", out string args);

            Assert.Equal(true, result);
            Assert.Equal(string.Empty, args);
        }
        
        [Fact]
        public void ProcessMessage_ParseCmdWithEmptyArgs()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo ", out string args);

            Assert.Equal(true, result);
            Assert.Equal(string.Empty, args);
        }
        
        [Fact]
        public void ProcessMessage_ReturnFalseIfWronCmd()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);

            var result = service.CheckAndTrimCommand("/todo", "/todo1 qwerty", out string args);

            Assert.Equal(false, result);
            Assert.Equal(string.Empty, args);
        }
    }
}