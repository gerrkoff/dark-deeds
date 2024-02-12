using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Implementation.CommandProcessor;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.CommandProcessor;

public class StartCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var telegramMock = new Mock<ITelegramService>();
        var sendMessageMock = new Mock<IBotSendMessageService>();
        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();

        var service = new StartCommandProcessor(sendMessageMock.Object, telegramMock.Object, loggerMock.Object);

        await service.ProcessAsync(new StartCommand("key")
        {
            UserChatId = 100,
        });

        telegramMock.Verify(x => x.UpdateChatId("key", 100));
        sendMessageMock.Verify(x => x.SendTextAsync(100, "Registered"));
    }
}
