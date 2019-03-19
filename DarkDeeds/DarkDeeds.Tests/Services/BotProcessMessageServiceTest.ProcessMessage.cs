using DarkDeeds.Models.Bot;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class BotProcessMessageServiceTest : BaseTest
    {
        [Fact]
        public void ProcessMessage_Todo_Processed()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);
            
            service.ProcessMessage(new UpdateDto
            {
                Message = new MessageDto
                {
                    Text = "/todo "
                }
            });

            sendMsgMock.Verify(x => x.SendText(It.Is<string>(s => s.StartsWith("Show todo"))));
            sendMsgMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public void ProcessMessage_NoCommand_Processed()
        {
            var sendMsgMock = new Mock<IBotSendMessageService>();
            var service = new BotProcessMessageService(sendMsgMock.Object);
            
            service.ProcessMessage(new UpdateDto
            {
                Message = new MessageDto
                {
                    Text = "Some strange string"
                }
            });

            sendMsgMock.Verify(x => x.SendText(It.Is<string>(s => s.StartsWith("Create task"))));
            sendMsgMock.VerifyNoOtherCalls();
        }
    }
}