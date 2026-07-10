using DD.Clients.Details.McpClient.Tools;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using Moq;
using Xunit;

namespace DD.Tests.Unit.McpClient;

public class AddTasksToolTests
{
    private readonly Mock<IMcpService> mcpServiceMock = new();
    private readonly Mock<IUserAuth> userAuthMock = new();

    [Fact]
    public async Task Do_WithValidTasksAndJustification_ForwardsToServiceWithUserIdAndReturnsResult()
    {
        // Arrange
        const string justification = "Added by agent";
        var tasks = new List<TaskCreateDto> { new() { Title = "Buy milk" } };
        userAuthMock.Setup(x => x.UserId()).Returns("user-1");
        mcpServiceMock
            .Setup(x => x.AddTasksAsync(tasks, "user-1", justification))
            .ReturnsAsync("result");

        // Act
        var result = await AddTasksTool.Do(mcpServiceMock.Object, userAuthMock.Object, tasks, justification);

        // Assert
        Assert.Equal("result", result);
        mcpServiceMock.Verify(x => x.AddTasksAsync(tasks, "user-1", justification), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Do_MissingJustification_ThrowsArgumentExceptionAndDoesNotCallService(string? justification)
    {
        // Arrange
        var tasks = new List<TaskCreateDto> { new() { Title = "Buy milk" } };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => AddTasksTool.Do(mcpServiceMock.Object, userAuthMock.Object, tasks, justification!));

        // Assert
        Assert.Equal("justification", exception.ParamName);
        mcpServiceMock.Verify(
            x => x.AddTasksAsync(It.IsAny<ICollection<TaskCreateDto>>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("empty")]
    [InlineData("null-element")]
    [InlineData("whitespace-title")]
    public async Task Do_InvalidTasks_ThrowsArgumentExceptionAndDoesNotCallService(string scenario)
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

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => AddTasksTool.Do(mcpServiceMock.Object, userAuthMock.Object, tasks!, "Added by agent"));

        // Assert
        Assert.Equal("tasks", exception.ParamName);
        mcpServiceMock.Verify(
            x => x.AddTasksAsync(It.IsAny<ICollection<TaskCreateDto>>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
