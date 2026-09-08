using System.Diagnostics.CodeAnalysis;
using DD.Shared.Details.Abstractions.Dto;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.TaskServiceTests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Tests")]
public partial class TaskServiceTest
{
    private const string UserId = "userid";
    private const string Uid = "uid";

    [Fact]
    public async Task SaveTasksAsync_ReturnTasksBack()
    {
        var service = CreateService();

        var items = new[] { new TaskDto { Id = 1000 }, new TaskDto { Id = 2000 } };
        var result = (await service.SaveTasksAsync(items, string.Empty, clientId: null)).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SaveTasksAsync_IgnoreAlreadyDeletedOnDelete()
    {
        var service = CreateService();

        var items = new[] { new TaskDto { Uid = Uid, Deleted = true } };
        var result = await service.SaveTasksAsync(items, UserId, clientId: null);

        Assert.Collection(result, _ => { });
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.VerifyNoOtherCalls();
    }
}
