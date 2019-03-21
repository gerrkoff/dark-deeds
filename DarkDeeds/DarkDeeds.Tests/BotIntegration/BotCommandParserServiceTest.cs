using System;
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
        public void ParseCommand_CreateTask()
        {
            var taskParserMock = new Mock<ITaskParserService>();
            taskParserMock.Setup(x => x.ParseTask(It.IsAny<string>())).Returns(new TaskDto());
            var service = new BotCommandParserService(taskParserMock.Object);

            var result = service.ParseCommand("Some task");

            Assert.IsType(typeof(CreateTaskCommand), result);
        }
        
        [Fact]
        public void ProcessMessage_ShowTodo()
        {
            var service = new BotCommandParserService(null);

            var result = service.ParseCommand("/todo");

            Assert.IsType(typeof(ShowTodoCommand), result);
            Assert.Equal(DateTime.Today, ((ShowTodoCommand) result).Day);
        }
    }
}