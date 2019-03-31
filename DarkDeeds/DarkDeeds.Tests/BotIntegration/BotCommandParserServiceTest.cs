using System;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Implementation;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration
{
    public class BotCommandParserServiceTest : BaseTest
    {
        [Fact]
        public async Task ParseCommand_CreateTask()
        {
            var task = new TaskDto();
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
            var taskParserMock = new Mock<ITaskParserService>();
            taskParserMock.Setup(x => x.ParseTask("Some task", 100)).Returns(task);
            var service = new BotCommandParserService(taskParserMock.Object, telegramMock.Object);

            var result = await service.ParseCommand("Some task", 100500);

            Assert.IsType<CreateTaskCommand>(result);
            Assert.Same(task, ((CreateTaskCommand) result).Task);
        }
        
        [Fact]
        public async Task ProcessMessage_ShowTodo()
        {
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
            var service = new BotCommandParserService(null, telegramMock.Object);

            var result = await service.ParseCommand("/todo", 100500);

            Assert.IsType<ShowTodoCommand>(result);
            Assert.Equal(DateTime.Today.AddMinutes(100), ((ShowTodoCommand) result).From);
            Assert.Equal(DateTime.Today.AddDays(1).AddMinutes(100), ((ShowTodoCommand) result).To);
        }
        
        [Fact]
        public async Task ProcessMessage_Start()
        {
            var service = new BotCommandParserService(null, null);

            var result = await service.ParseCommand("/start SomeChatKey", 0);

            Assert.IsType<StartCommand>(result);
            Assert.Equal("SomeChatKey", ((StartCommand) result).UserChatKey);
        }
    }
}