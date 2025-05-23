using DD.ServiceTask.Domain.Entities;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.TaskServiceTests;

public partial class TaskServiceTest
{
    [Fact]
    public async Task LoadTasksByDateAsync_Positive()
    {
        var userId = "userid";
        var from = new DateTime(2018, 10, 20);
        var to = new DateTime(2018, 10, 26);

        var service = CreateService(new TaskEntity { Date = new DateTime(2018, 10, 10) });

        _ = (await service.LoadTasksByDateAsync(userId, from, to)).ToList();

        // TODO: is it needed?
        // Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue).Date!.Value.Kind);
        _taskSpecMock.Verify(x => x.FilterUserOwned(userId));
        _taskSpecMock.Verify(x => x.FilterDateInterval(from, to));
        _repoMock.Verify(x => x.GetBySpecAsync(_taskSpecMock.Object));
    }
}
