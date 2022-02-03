using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using Moq;
using Xunit;

namespace DarkDeeds.TelegramClient.Tests.Services.CommandProcessor
{
    public class StartCommandProcessorTest
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