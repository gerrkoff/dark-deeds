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
        const string justification = "Reordered by priority";
        var updates = new List<TaskUpdateDto> { new() { Uid = "uid-1", Order = 2 } };
        IEnumerable<TaskDto> resultTasks =
            [new() { Uid = "uid-1", Order = 2, Type = TaskTypeDto.Routine }];
        taskServiceAppMock
            .Setup(x => x.UpdateTasksAsync(updates, "user-1", null))
            .ReturnsAsync(resultTasks);
        loggerMock
            .Setup(x => x.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);
        var service = CreateService();

        // Act
        var result = await service.UpdateTasksOrderAsync(updates, "user-1", justification);

        // Assert
        taskServiceAppMock.Verify(x => x.UpdateTasksAsync(updates, "user-1", null), Times.Once);
        Assert.Equal(Serialize(resultTasks), result);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(justification)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
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

    [Fact]
    public async Task AddTasksAsync_WithTasksAndJustification_SavesMappedTasksAndReturnsSerializedResult()
    {
        // Arrange
        const string justification = "Added by agent";
        var taskToCreate = new TaskCreateDto
        {
            Title = "Buy milk",
            Date = new DateTime(2026, 7, 10),
            Time = 1050,
            Type = TaskTypeDto.Routine,
            IsProbable = true,
        };
        IEnumerable<TaskDto> savedResult =
            [new() { Uid = "uid-1", Title = "Buy milk", Type = TaskTypeDto.Routine }];
        ICollection<TaskDto>? capturedTasks = null;
        taskServiceAppMock
            .Setup(x => x.SaveTasksAsync(It.IsAny<ICollection<TaskDto>>(), "user-1", null))
            .Callback<ICollection<TaskDto>, string, string?>((tasks, _, _) => capturedTasks = tasks)
            .ReturnsAsync(savedResult);
        loggerMock
            .Setup(x => x.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);
        var service = CreateService();

        // Act
        var result = await service.AddTasksAsync([taskToCreate], "user-1", justification);

        // Assert
        taskServiceAppMock.Verify(x => x.SaveTasksAsync(It.IsAny<ICollection<TaskDto>>(), "user-1", null), Times.Once);
        Assert.NotNull(capturedTasks);
        var savedTask = Assert.Single(capturedTasks);
        Assert.False(string.IsNullOrEmpty(savedTask.Uid));
        Assert.Equal(taskToCreate.Title, savedTask.Title);
        Assert.Equal(taskToCreate.Date, savedTask.Date);
        Assert.Equal(taskToCreate.Time, savedTask.Time);
        Assert.Equal(taskToCreate.Type, savedTask.Type);
        Assert.Equal(taskToCreate.IsProbable, savedTask.IsProbable);
        Assert.Equal(Serialize(savedResult), result);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(justification)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddTasksAsync_MissingJustification_ThrowsArgumentException(string? justification)
    {
        // Arrange
        var tasks = new List<TaskCreateDto> { new() { Title = "Buy milk" } };
        var service = CreateService();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddTasksAsync(tasks, "user-1", justification!));

        // Assert
        Assert.Equal("justification", exception.ParamName);
        taskServiceAppMock.Verify(
            x => x.SaveTasksAsync(It.IsAny<ICollection<TaskDto>>(), It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("empty")]
    [InlineData("null-element")]
    [InlineData("whitespace-title")]
    public async Task AddTasksAsync_InvalidTasks_ThrowsArgumentException(string scenario)
    {
        // Arrange
        ICollection<TaskCreateDto>? tasks = scenario switch
        {
            "null" => null,
            "empty" => [],
            "null-element" => [null!],
            "whitespace-title" => [new() { Title = "   " }],
            _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
        };
        var service = CreateService();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddTasksAsync(tasks!, "user-1", "Added by agent"));

        // Assert
        Assert.Equal("tasks", exception.ParamName);
        taskServiceAppMock.Verify(
            x => x.SaveTasksAsync(It.IsAny<ICollection<TaskDto>>(), It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
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
