using System;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Implementation.CommandProcessor;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration.CommandProcessor
{
    public class BaseCommandProcessorTest : BaseTest
    {
        class BotCommandImplementation : BotCommand
        {
        }
        
        class BaseCommandProcessorImplementation : BaseCommandProcessor<BotCommandImplementation>
        {
            private readonly bool _throwException;

            public BaseCommandProcessorImplementation(
                IBotSendMessageService botSendMessageService,
                ILogger<BaseCommandProcessor<BotCommand>> logger,
                bool throwException) : base(botSendMessageService, logger)
            {
                _throwException = throwException;
            }

            protected override Task ProcessCoreAsync(BotCommandImplementation command)
            {
                if (_throwException)
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
            
            // TODO: uncomment
//            loggerMock.Verify(x => x.LogWarning(It.IsAny<string>()));
            sendMessageMock.Verify(x => x.SendFailedAsync(100));
        }
    }
}