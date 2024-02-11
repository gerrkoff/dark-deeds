using System.Diagnostics.CodeAnalysis;
using DD.TelegramClient.Domain.Implementation;
using DD.TelegramClient.Domain.Implementation.CommandProcessor;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.TelegramClient.Tests.Unit.Services.CommandProcessor;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class BaseCommandProcessorTest
{
    private sealed class BotCommandImplementation : BotCommand;

    private sealed class BaseCommandProcessorImplementation(
        IBotSendMessageService botSendMessageService,
        ILogger<BaseCommandProcessor<BotCommand>> logger,
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        bool throwException)
        : BaseCommandProcessor<BotCommandImplementation>(botSendMessageService, logger)
    {
        protected override Task ProcessCoreAsync(BotCommandImplementation command)
        {
            return throwException
                ? throw new InvalidOperationException()
                : Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ProcessAsync_ProcessSuccess()
    {
        var botSendMessageServiceMock = new Mock<IBotSendMessageService>();
        var loggerMock = new Mock<ILogger<BaseCommandProcessor<BotCommand>>>();
        var service = new BaseCommandProcessorImplementation(botSendMessageServiceMock.Object, loggerMock.Object, false);

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
