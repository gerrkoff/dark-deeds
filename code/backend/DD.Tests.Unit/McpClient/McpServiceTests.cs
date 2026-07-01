using System.Text.Json;
using System.Text.Json.Serialization;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DD.Tests.Unit.McpClient;

public class McpServiceTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly Mock<ITaskServiceApp> taskServiceAppMock = new();
    private readonly Mock<ILogger<McpService>> loggerMock = new();

    [Fact]
    public async Task UpdateTasksOrderAsync_WithJustification_ForwardsUpdatesAndReturnsSerializedResult()
    {
        // Arrange
        var updates = new List<TaskUpdateDto> { new() { Uid = "uid-1", Order = 2 } };
        IEnumerable<TaskDto> resultTasks =
            [new() { Uid = "uid-1", Order = 2, Type = TaskTypeDto.Routine }];
        taskServiceAppMock
            .Setup(x => x.UpdateTasksAsync(updates, "user-1", null))
            .ReturnsAsync(resultTasks);
        var service = CreateService();

        // Act
        var result = await service.UpdateTasksOrderAsync(updates, "user-1", "Reordered by priority");

        // Assert
        taskServiceAppMock.Verify(x => x.UpdateTasksAsync(updates, "user-1", null), Times.Once);
        Assert.Equal(Serialize(resultTasks), result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateTasksOrderAsync_MissingJustification_ThrowsArgumentException(string? justification)
    {
        // Arrange
        var updates = new List<TaskUpdateDto> { new() { Uid = "uid-1", Order = 1 } };
        var service = CreateService();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateTasksOrderAsync(updates, "user-1", justification!));

        // Assert
        Assert.Equal("justification", exception.ParamName);
        taskServiceAppMock.Verify(
            x => x.UpdateTasksAsync(It.IsAny<ICollection<TaskUpdateDto>>(), It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public async Task LoadTasksByDateAsync_WithDateRange_ForwardsArgumentsAndReturnsSerializedResult()
    {
        // Arrange
        var from = new DateTime(2026, 7, 1);
        var till = new DateTime(2026, 7, 14);
        IEnumerable<TaskDto> resultTasks =
            [new() { Uid = "uid-1", Title = "Task", Type = TaskTypeDto.Weekly }];
        taskServiceAppMock
            .Setup(x => x.LoadTasksByDateAsync(from, till, "user-1"))
            .ReturnsAsync(resultTasks);
        var service = CreateService();

        // Act
        var result = await service.LoadTasksByDateAsync(from, till, "user-1");

        // Assert
        taskServiceAppMock.Verify(x => x.LoadTasksByDateAsync(from, till, "user-1"), Times.Once);
        Assert.Equal(Serialize(resultTasks), result);
    }

    private static string Serialize(IEnumerable<TaskDto> tasks)
    {
        return JsonSerializer.Serialize(tasks, JsonOptions);
    }

    private McpService CreateService()
    {
        return new(taskServiceAppMock.Object, loggerMock.Object);
    }
}
