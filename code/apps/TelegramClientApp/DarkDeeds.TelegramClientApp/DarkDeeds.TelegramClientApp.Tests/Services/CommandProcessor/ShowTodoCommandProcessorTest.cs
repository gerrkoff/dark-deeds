using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Services.Interface;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.TelegramClientApp.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Services.Models.Commands;
using Moq;
using Xunit;

namespace DarkDeeds.TelegramClientApp.Tests.Services.CommandProcessor
{
    public class ShowTodoCommandProcessorTest
    {
        [Fact]
        public async Task ProcessAsync()
        {
            var (telegramMock, taskServiceMock, taskParserMock, sendMessageMock) = SetupMocks("tasks", 100);
            
            var service = new ShowTodoCommandProcessor(
                sendMessageMock.Object,
                telegramMock.Object,
                null,
                taskParserMock.Object);

            await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
            {
                UserChatId = 100
            });


            sendMessageMock.Verify(x => x.SendTextAsync(100, "tasks"));
        }
        
        [Fact]
        public async Task ProcessAsync_NoTasks()
        {
            var (telegramMock, taskServiceMock, taskParserMock, sendMessageMock) = SetupMocks(string.Empty, 100);
            
            var service = new ShowTodoCommandProcessor(
                sendMessageMock.Object,
                telegramMock.Object,
                null,
                taskParserMock.Object);

            await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
            {
                UserChatId = 100
            });


            sendMessageMock.Verify(x => x.SendTextAsync(100, "No tasks"));
        }

        private (Mock<ITelegramService>, Mock<ITaskServiceApp>, Mock<ITaskServiceApp>, Mock<IBotSendMessageService>)
            SetupMocks(string tasksAsString, int chatId)
        {
            var tasks = new TaskDto[0];
            
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserId(chatId))
                .Returns(Task.FromResult("userid"));
            
            var taskServiceMock = new Mock<ITaskServiceApp>();
            taskServiceMock.Setup(x => x.LoadTasksByDateAsync("userid", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((IEnumerable<TaskDto>) tasks));
            
            var taskParserMock = new Mock<ITaskServiceApp>();
            taskParserMock.Setup(x => x.PrintTasks(tasks))
                .Returns(Task.FromResult(tasksAsString));
            
            var sendMessageMock = new Mock<IBotSendMessageService>();

            return (telegramMock, taskServiceMock, taskParserMock, sendMessageMock);
        } 
    }
}