using System;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DarkDeeds.ServiceTask.Dto;
using DarkDeeds.TelegramClient.Services.Implementation;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using Moq;
using Xunit;

namespace DarkDeeds.TelegramClient.Tests.Services
{
    public class BotCommandParserServiceTest
    {
        [Fact]
        public async Task ParseCommand_CreateTask()
        {
            var task = new TaskDto();
            var telegramMock = new Mock<ITelegramService>();
            var taskParserMock = new Mock<ITaskServiceApp>();
            taskParserMock.Setup(x => x.ParseTask("Some task")).Returns(Task.FromResult(task));
            var service = new BotCommandParserService(telegramMock.Object, null, taskParserMock.Object);

            var result = await service.ParseCommand("Some task", 100500);

            Assert.IsType<CreateTaskCommand>(result);
            Assert.Same(task, ((CreateTaskCommand) result).Task);
        }

        [Fact]
        public async Task ProcessMessage_ShowTodo()
        {
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2010, 10, 10));
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
            var service = new BotCommandParserService(telegramMock.Object, dateServiceMock.Object, null);

            var result = await service.ParseCommand("/todo", 100500);

            Assert.IsType<ShowTodoCommand>(result);
            Assert.Equal(new DateTime(2010, 10, 10), ((ShowTodoCommand) result).From);
            Assert.Equal(new DateTime(2010, 10, 11), ((ShowTodoCommand) result).To);
        }

        [Fact]
        public async Task ProcessMessage_Start()
        {
            var service = new BotCommandParserService(null, null, null);

            var result = await service.ParseCommand("/start SomeChatKey", 0);

            Assert.IsType<StartCommand>(result);
            Assert.Equal("SomeChatKey", ((StartCommand) result).UserChatKey);
        }
    }
}
