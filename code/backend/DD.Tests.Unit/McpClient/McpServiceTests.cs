using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.Tests.Unit.McpClient;

public class McpServiceTests
{
    private readonly Mock<ITaskServiceApp> taskServiceAppMock = new();
    private readonly Mock<ILogger<McpService>> loggerMock = new();

    [Fact]
    public async Task UpdateTasksOrderAsync_WithJustification_LogsJustification()
    {
        const string justification = "Reordered by priority";
        loggerMock
            .Setup(x => x.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);
        var service = CreateService();

        await service.UpdateTasksOrderAsync(
            [new TaskUpdateDto { Uid = "uid-1", Order = 2 }],
            "user-1",
            justification);

        VerifyJustificationLogged(justification);
    }

    [Fact]
    public async Task AddTasksAsync_WithJustification_LogsJustification()
    {
        const string justification = "Added by agent";
        loggerMock
            .Setup(x => x.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);
        var service = CreateService();

        await service.AddTasksAsync(
            [new TaskCreateDto { Title = "Buy milk" }],
            "user-1",
            justification);

        VerifyJustificationLogged(justification);
    }

    private McpService CreateService()
    {
        return new(taskServiceAppMock.Object, loggerMock.Object);
    }

    private void VerifyJustificationLogged(string justification)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(justification)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
