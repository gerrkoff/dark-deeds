using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.Shared.Details.Abstractions.Dto;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest
{
    [Fact]
    public async Task CreateAsync_DoNothingIfNoNonDeletedRecurrences()
    {
        var service = Service();

        await service.CreateAsync(0, string.Empty);

        _plannedRecurrenceRepoMock.Verify(x => x.GetBySpecAsync(_plannedRecurrenceSpecMock.Object));
        _taskRepoMock.VerifyNoOtherCalls();
        _plannedRecurrenceRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_CreateTaskForRecurrence()
    {
        var someDate = DateTime.UtcNow.Date;
        _dateServiceMock.SetupGet(x => x.Now).Returns(someDate);

        var service = Service(new PlannedRecurrenceEntity
        {
            Task = "Task",
            EveryNthDay = 1,
            StartDate = someDate,
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Title == "Task" && y.UserId == "userId" && y.Date == someDate)));
    }

    [Fact]
    public async Task CreateAsync_CreateRecurrenceTaskForRecurrence()
    {
        var now = new DateTime(2019, 10, 10);
        _taskRepoMock
            .Setup(x => x.UpsertAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.CompletedTask)
            .Callback<TaskEntity>(x => x.Uid = "uid");
        _dateServiceMock.SetupGet(x => x.Now).Returns(now);

        var service = Service(new PlannedRecurrenceEntity
        {
            Task = "Task",
            EveryNthDay = 1,
            StartDate = now,
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        // Verify that TryAddRecurrenceAsync was called for the date
        _plannedRecurrenceRepoMock.Verify(
            x => x.TryAddRecurrenceAsync(
                It.IsAny<string>(),
                It.Is<RecurrenceEntity>(r => r.DateTime == now)),
            Times.AtLeastOnce);

        // Verify that TryUpdateRecurrenceTaskUidAsync was called with the task UID
        _plannedRecurrenceRepoMock.Verify(
            x => x.TryUpdateRecurrenceTaskUidAsync(
                It.IsAny<string>(),
                now,
                "uid"),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task CreateAsync_CreateEntitiesExpectedNumberOfTimes()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));

        var service = Service(new PlannedRecurrenceEntity
        {
            EveryNthDay = 1,
            StartDate = new DateTime(2019, 9, 3),
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.IsAny<TaskEntity>()), Times.Exactly(13));
    }

    [Fact]
    public async Task CreateAsync_SimpleTestWeekday()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));

        var service = Service(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 6),
            EveryWeekday = RecurrenceWeekday.Monday | RecurrenceWeekday.Wednesday,
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 9))));
        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 11))));
        _taskRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_SimpleTestMonthDay()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));

        var service = Service(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 6),
            EveryMonthDay = "9,11",
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 9))));
        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 11))));
        _taskRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_SimpleTestNthDay()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));

        var service = Service(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 6),
            EveryNthDay = 6,
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 6))));
        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 12))));
        _taskRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_WeekdayAndMonthDayAndNthDayAtTheSameTime()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));

        var service = Service(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 6),
            EveryNthDay = 6,
            EveryMonthDay = "6,12,13",
            EveryWeekday = RecurrenceWeekday.Thursday | RecurrenceWeekday.Wednesday,
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.Is<TaskEntity>(
            y => y.Date == new DateTime(2019, 9, 12))));
        _taskRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_IgnoreAlreadyExistingRecurrences()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));

        var service = Service(new PlannedRecurrenceEntity
        {
            EveryNthDay = 1,
            StartDate = new DateTime(2019, 9, 3),
            UserId = "userId",
            Recurrences = [new RecurrenceEntity { DateTime = new DateTime(2019, 9, 3) }],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.IsAny<TaskEntity>()), Times.Exactly(12));
    }

    [Fact]
    public async Task CreateAsync_FilterPlannedRecurrencesByUser()
    {
        var service = Service();

        await service.CreateAsync(0, "userId100500");

        _plannedRecurrenceRepoMock.Verify(x => x.GetBySpecAsync(_plannedRecurrenceSpecMock.Object));
        _plannedRecurrenceSpecMock.Verify(x => x.FilterUserOwned("userId100500"));
    }

    [Fact]
    public async Task CreateAsync_NoRepeats()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));

        var service = Service(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 6),
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _taskRepoMock.Verify(x => x.UpsertAsync(It.IsAny<TaskEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NotifyAboutCreatedTasks()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));

        var service = Service(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 6),
            EveryMonthDay = "6",
            UserId = "userId",
            Recurrences = [],
        });

        await service.CreateAsync(0, "userId");

        _notifierServiceMock.Verify(x => x.TaskUpdated(
            It.Is<TasksUpdatedDto>(y => y.Tasks.Count == 1 && y.Tasks.First().Title == "Task")));
    }
}
