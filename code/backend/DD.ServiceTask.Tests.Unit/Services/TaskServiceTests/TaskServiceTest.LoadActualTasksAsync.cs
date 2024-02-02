using DD.ServiceTask.Domain.Entities;
using Xunit;

namespace DD.ServiceTask.Tests.Unit.Services.TaskServiceTests;

public partial class TaskServiceTest
{
    [Fact]
    public async Task LoadActualTasksAsync_Positive()
    {
        var userId = "userid";
        var from = new DateTime(2000, 1, 1);

        CreateService(new TaskEntity {Date = new DateTime(2018, 10, 10)});

        var result = (await _service.LoadActualTasksAsync(userId, from)).ToList();

        Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue)!.Date!.Value.Kind);
        _taskSpecMock.Verify(x => x.FilterUserOwned(userId));
        _taskSpecMock.Verify(x => x.FilterActual(from));
        _repoMock.Verify(x => x.GetBySpecAsync(_taskSpecMock.Object));
    }
}
