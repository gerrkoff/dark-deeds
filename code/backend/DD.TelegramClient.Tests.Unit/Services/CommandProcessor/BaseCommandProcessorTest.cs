using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Implementation.CommandProcessor;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.CommandProcessor;

public class BaseCommandProcessorTest
{
    class BotCommandImplementation : BotCommand;

    class BaseCommandProcessorImplementation(
        IBotSendMessageService botSendMessageService,
        ILogger<BaseCommandProcessor<BotCommand>> logger,
        bool throwException)
        : BaseCommandProcessor<BotCommandImplementation>(botSendMessageService, logger)
    {
        protected override Task ProcessCoreAsync(BotCommandImplementation command)
        {
            if (throwException)
                throw new Exception();
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ProcessAsync_ProcessSuccess()
    {
        var service = new BaseCommandProcessorImplementation(null, null, false);

        await service.ProcessAsync(new BotCommandImplementation());
    }

    [Fact]
    public async Task ProcessAsync_ProcessFail()
    {
        var sendMessageMock = new Mock<IBotSendMessageService>();
        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();
        var service = new BaseCommandProcessorImplementation(sendMessageMock.Object, loggerMock.Object, true);

        await service.ProcessAsync(new BotCommandImplementation
        {
            UserChatId = 100
        });

        sendMessageMock.Verify(x => x.SendFailedAsync(100));
    }
}
