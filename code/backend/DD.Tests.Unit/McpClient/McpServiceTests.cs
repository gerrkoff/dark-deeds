using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.Tests.Unit.McpClient;

public class McpServiceTests
{
    private readonly Mock<ITaskServiceApp> _taskServiceAppMock = new();
    private readonly Mock<ILogger<McpService>> _loggerMock = new();

    [Fact]
    public async Task UpdateTasksOrderAsync_WithJustification_LogsJustification()
    {
        const string justification = "Reordered by priority";
        _loggerMock
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
        _loggerMock
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
        return new(_taskServiceAppMock.Object, _loggerMock.Object);
    }

    private void VerifyJustificationLogged(string justification)
    {
        var invocation = Assert.Single(
            _loggerMock.Invocations,
            invocation => invocation.Method.Name == nameof(ILogger.Log));
        Assert.Equal(LogLevel.Information, invocation.Arguments[0]);

        var message = invocation.Arguments[2].ToString();
        Assert.NotNull(message);
        Assert.Contains(justification, message, StringComparison.Ordinal);
    }
}
