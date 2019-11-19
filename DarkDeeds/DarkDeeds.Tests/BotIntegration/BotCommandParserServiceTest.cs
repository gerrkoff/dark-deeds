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
            var taskParserMock = new Mock<ITaskParserService>();
            taskParserMock.Setup(x => x.ParseTask("Some task", It.IsAny<bool>())).Returns(task);
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
            Assert.Equal(DateTime.UtcNow.AddMinutes(100).Date, ((ShowTodoCommand) result).From);
            Assert.Equal(DateTime.UtcNow.AddMinutes(100).AddDays(1).Date, ((ShowTodoCommand) result).To);
        }
        
        [Fact]
        public async Task ProcessMessage_ShowTodo_WithDate()
        {
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
            var service = new BotCommandParserService(null, telegramMock.Object);

            var result = await service.ParseCommand("/todo 1010", 100500);

            var date = new DateTime(DateTime.UtcNow.AddMinutes(100).Year, 10, 10);
            Assert.IsType<ShowTodoCommand>(result);
            Assert.Equal(date, ((ShowTodoCommand) result).From);
            Assert.Equal(date.AddDays(1), ((ShowTodoCommand) result).To);
        }
        
        [Fact]
        public async Task ProcessMessage_ShowTodo_WithDateAndYear()
        {
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserTimeAdjustment(100500)).Returns(Task.FromResult(100));
            var service = new BotCommandParserService(null, telegramMock.Object);

            var result = await service.ParseCommand("/todo 20161010", 100500);

            var date = new DateTime(2016, 10, 10);
            Assert.IsType<ShowTodoCommand>(result);
            Assert.Equal(date, ((ShowTodoCommand) result).From);
            Assert.Equal(date.AddDays(1), ((ShowTodoCommand) result).To);
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