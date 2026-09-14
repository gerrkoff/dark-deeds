using DD.Clients.Details.McpClient.Tools;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using Moq;
using Xunit;

namespace DD.Tests.Unit.McpClient;

public class UpdateTasksOrderToolTests
{
    private readonly Mock<IMcpService> _mcpServiceMock = new();
    private readonly Mock<IUserAuth> _userAuthMock = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Do_MissingJustification_ThrowsArgumentExceptionAndDoesNotCallService(string? justification)
    {
        // Arrange
        var updates = new List<TaskUpdateDto?> { new() { Uid = "uid-1", Order = 1 } };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => UpdateTasksOrderTool.Do(_mcpServiceMock.Object, _userAuthMock.Object, updates, justification!));

        // Assert
        Assert.Equal("justification", exception.ParamName);
        _mcpServiceMock.Verify(
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
        ICollection<TaskUpdateDto?>? updates = scenario switch
        {
            "null" => null,
            "empty" => [],
            "null-element" => [null],
            "whitespace-uid" => [new() { Uid = "   " }],
            _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => UpdateTasksOrderTool.Do(
                _mcpServiceMock.Object,
                _userAuthMock.Object,
                updates,
                "Reordered by priority"));

        // Assert
        Assert.Equal("updates", exception.ParamName);
        _mcpServiceMock.Verify(
            x => x.UpdateTasksOrderAsync(It.IsAny<ICollection<TaskUpdateDto>>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
