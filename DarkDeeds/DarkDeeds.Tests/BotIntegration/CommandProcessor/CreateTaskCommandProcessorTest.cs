using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Implementation.CommandProcessor;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Dto;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.BotIntegration.CommandProcessor
{
    public class CreateTaskCommandProcessorTest : BaseTest
    {
        [Fact]
        public async Task ProcessAsync()
        {
            var task = new TaskDto();
            var tasks = new TaskDto[0];
            var (telegramMock, taskServiceMock, sendMessageMock, taskHubService) = SetupMocks(task, tasks, 100);
            
            var service = new CreateTaskCommandProcessor(
                sendMessageMock.Object,
                telegramMock.Object,
                taskServiceMock.Object,
                taskHubService.Object,
                null);

            await service.ProcessAsync(new CreateTaskCommand(task)
            {
                UserChatId = 100
            });


            sendMessageMock.Verify(x => x.SendTextAsync(100, "Task created"));
        }
        
        [Fact]
        public async Task ProcessAsync_SendUpdateTasks()
        {
            var task = new TaskDto();
            var tasks = new TaskDto[0];
            var (telegramMock, taskServiceMock, sendMessageMock, taskHubService) = SetupMocks(task, tasks, 100);
            
            var service = new CreateTaskCommandProcessor(
                sendMessageMock.Object,
                telegramMock.Object,
                taskServiceMock.Object,
                taskHubService.Object,
                null);

            await service.ProcessAsync(new CreateTaskCommand(task)
            {
                UserChatId = 100
            });

            taskHubService.Verify(x => x.Update(
                It.Is<IEnumerable<TaskDto>>(y => y == tasks)
            ));
        }

        private (Mock<ITelegramService>, Mock<ITaskService>, Mock<IBotSendMessageService>, Mock<ITaskHubService>)
            SetupMocks(TaskDto task, TaskDto[] tasks, int chatId)
        {   
            var telegramMock = new Mock<ITelegramService>();
            telegramMock.Setup(x => x.GetUserId(chatId))
                .Returns(Task.FromResult("userid"));
            
            var taskServiceMock = new Mock<ITaskService>();
            taskServiceMock.Setup(x => x.SaveTasksAsync(It.Is<ICollection<TaskDto>>(v => v.Contains(task)), "userid"))
                .Returns(Task.FromResult((IEnumerable<TaskDto>) tasks));
            
            var sendMessageMock = new Mock<IBotSendMessageService>();

            var taskHubServiceMock = new Mock<ITaskHubService>();

            return (telegramMock, taskServiceMock, sendMessageMock, taskHubServiceMock);
        } 
    }
}