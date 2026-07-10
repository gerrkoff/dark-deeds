using DD.Clients.Details.McpClient.Tools;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using Moq;
using Xunit;

namespace DD.Tests.Unit.McpClient;

public class UpdateTasksOrderToolTests
{
    private readonly Mock<IMcpService> mcpServiceMock = new();
    private readonly Mock<IUserAuth> userAuthMock = new();

    [Fact]
    public async Task Do_WithValidUpdatesAndJustification_ForwardsToServiceWithUserIdAndReturnsResult()
    {
        // Arrange
        const string justification = "Reordered by priority";
        var updates = new List<TaskUpdateDto> { new() { Uid = "uid-1", Order = 2 } };
        userAuthMock.Setup(x => x.UserId()).Returns("user-1");
        mcpServiceMock
            .Setup(x => x.UpdateTasksOrderAsync(updates, "user-1", justification))
            .ReturnsAsync("result");

        // Act
        var result = await UpdateTasksOrderTool.Do(mcpServiceMock.Object, userAuthMock.Object, updates, justification);

        // Assert
        Assert.Equal("result", result);
        mcpServiceMock.Verify(x => x.UpdateTasksOrderAsync(updates, "user-1", justification), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Do_MissingJustification_ThrowsArgumentExceptionAndDoesNotCallService(string? justification)
    {
        // Arrange
        var updates = new List<TaskUpdateDto> { new() { Uid = "uid-1", Order = 1 } };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => UpdateTasksOrderTool.Do(mcpServiceMock.Object, userAuthMock.Object, updates, justification!));

        // Assert
        Assert.Equal("justification", exception.ParamName);
        mcpServiceMock.Verify(
            x => x.UpdateTasksOrderAsync(It.IsAny<ICollection<TaskUpdateDto>>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("empty")]
    [InlineData("null-element")]
    [InlineData("whitespace-uid")]
    public async Task Do_InvalidUpdates_ThrowsArgumentExceptionAndDoesNotCallService(string scenario)
    {
        // Arrange
        ICollection<TaskUpdateDto>? updates = scenario switch
        {
            "null" => null,
            "empty" => [],
            "null-element" => [null!],
            "whitespace-uid" => [new() { Uid = "   " }],
            _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => UpdateTasksOrderTool.Do(mcpServiceMock.Object, userAuthMock.Object, updates!, "Reordered by priority"));

        // Assert
        Assert.Equal("updates", exception.ParamName);
        mcpServiceMock.Verify(
            x => x.UpdateTasksOrderAsync(It.IsAny<ICollection<TaskUpdateDto>>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
