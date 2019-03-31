using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Implementation.CommandProcessor;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration.CommandProcessor
{
    public class StartCommandProcessorTest : BaseTest
    {
        [Fact]
        public async Task ProcessAsync()
        {
            var telegramMock = new Mock<ITelegramService>();
            var sendMessageMock = new Mock<IBotSendMessageService>();

            var service = new StartCommandProcessor(sendMessageMock.Object, telegramMock.Object, null);

            await service.ProcessAsync(new StartCommand("key")
            {
                UserChatId = 100
            });

            telegramMock.Verify(x => x.UpdateChatId("key", 100));
            sendMessageMock.Verify(x => x.SendTextAsync(100, "Registered"));
        } 
    }
}