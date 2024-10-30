using System.Diagnostics.CodeAnalysis;
using DD.ServiceTask.Domain.Entities;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.TaskServiceTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public partial class TaskServiceTest
{
    [Fact]
    public async Task LoadActualTasksAsync_Positive()
    {
        var userId = "userid";
        var from = new DateTime(2000, 1, 1);

        var service = CreateService(new TaskEntity { Date = new DateTime(2018, 10, 10) });

        _ = (await service.LoadActualTasksAsync(userId, from)).ToList();

        // TODO: is it needed?
        // Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue).Date!.Value.Kind);
        _taskSpecMock.Verify(x => x.FilterUserOwned(userId));
        _taskSpecMock.Verify(x => x.FilterActual(from));
        _repoMock.Verify(x => x.GetBySpecAsync(_taskSpecMock.Object));
    }
}
