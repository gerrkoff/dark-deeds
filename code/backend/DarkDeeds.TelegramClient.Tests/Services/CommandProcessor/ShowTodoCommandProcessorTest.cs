using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using DD.TaskService.Domain.Dto;
using Moq;
using Xunit;

namespace DarkDeeds.TelegramClient.Tests.Services.CommandProcessor;

public class ShowTodoCommandProcessorTest
{
    [Fact]
    public async Task ProcessAsync()
    {
        var (telegramMock, taskServiceMock, sendMessageMock) = SetupMocks(new[]{"tasks"}, 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            null,
            taskServiceMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "tasks"));
    }

    [Fact]
    public async Task ProcessAsync_NoTasks()
    {
        var (telegramMock, taskServiceMock, sendMessageMock) = SetupMocks(new string[0], 100);

        var service = new ShowTodoCommandProcessor(
            sendMessageMock.Object,
            telegramMock.Object,
            null,
            taskServiceMock.Object);

        await service.ProcessAsync(new ShowTodoCommand(string.Empty, new DateTime(), 0)
        {
            UserChatId = 100
        });

        sendMessageMock.Verify(x => x.SendTextAsync(100, "No tasks"));
    }

    private (Mock<ITelegramService>, Mock<ITaskServiceApp>, Mock<IBotSendMessageService>)
        SetupMocks(ICollection<string> tasksAsString, int chatId)
    {
        var tasks = new TaskDto[0];

        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(x => x.GetUserId(chatId))
            .Returns(Task.FromResult("userid"));

        var taskServiceMock = new Mock<ITaskServiceApp>();
        taskServiceMock.Setup(x => x.PrintTasks(tasks))
            .Returns(Task.FromResult(tasksAsString));

        var sendMessageMock = new Mock<IBotSendMessageService>();

        return (telegramMock, taskServiceMock, sendMessageMock);
    }
}
