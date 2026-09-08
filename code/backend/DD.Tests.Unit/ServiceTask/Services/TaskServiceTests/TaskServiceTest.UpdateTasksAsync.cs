using System.Diagnostics.CodeAnalysis;
using DD.ServiceTask.Domain.Entities;
using DD.Shared.Details.Abstractions.Dto;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.TaskServiceTests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Tests")]
public partial class TaskServiceTest
{
    [Fact]
    public async Task UpdateTasksAsync_EmptyCollection_ReturnsEmpty()
    {
        var userId = "user123";
        var service = CreateService();

        var updates = Array.Empty<TaskUpdateDto>();

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Empty(result);
        _repoMock.VerifyNoOtherCalls();
        _notifierServiceMock.Verify(x => x.TaskUpdated(It.IsAny<TasksUpdatedDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTasksAsync_AllUpdatesFail_ReturnsEmpty()
    {
        var uid1 = "uid1";
        var uid2 = "uid2";
        var userId = "user123";

        var entity1 = new TaskEntity { Uid = uid1, UserId = userId, Order = 1, Version = 1 };
        var entity2 = new TaskEntity { Uid = uid2, UserId = userId, Order = 2, Version = 1 };
        var service = CreateService(entity1, entity2);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((false, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = uid1, Order = 10 },
            new TaskUpdateDto { Uid = uid2, Order = 20 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Empty(result);
        _repoMock.Verify(x => x.GetByIdAsync(uid1), Times.Once);
        _repoMock.Verify(x => x.GetByIdAsync(uid2), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()), Times.Exactly(2));
        _notifierServiceMock.Verify(x => x.TaskUpdated(It.IsAny<TasksUpdatedDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTasksAsync_UpdatesOrderCorrectly()
    {
        var uid = "uid1";
        var userId = "user123";
        var originalOrder = 5;
        var newOrder = 15;

        var entity = new TaskEntity { Uid = uid, UserId = userId, Order = originalOrder, Version = 1 };
        var service = CreateService(entity);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[] { new TaskUpdateDto { Uid = uid, Order = newOrder } };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Single(result);
        Assert.Equal(uid, result[0].Uid);
        Assert.Equal(newOrder, result[0].Order);

        _repoMock.Verify(
            x => x.TryUpdateVersionAsync(
                It.Is<TaskEntity>(e =>
                    e.Uid == uid && e.Order == newOrder)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTasksAsync_NotifiesOnSuccess()
    {
        var uid = "uid1";
        var userId = "user123";

        var entity = new TaskEntity { Uid = uid, UserId = userId, Order = 1, Version = 1 };
        var service = CreateService(entity);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[] { new TaskUpdateDto { Uid = uid, Order = 10 } };

        await service.UpdateTasksAsync(updates, userId, clientId: null);

        _notifierServiceMock.Verify(
            x => x.TaskUpdated(
                It.Is<TasksUpdatedDto>(dto =>
                    dto.UserId == userId &&
                    dto.Tasks.Count() == 1 &&
                    dto.Tasks.First().Uid == uid)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTasksAsync_DoesNotNotifyOnNoSuccessfulUpdates()
    {
        var nonExistentUid = "nonexistent";
        var userId = "user123";
        var service = CreateService();

        var updates = new[] { new TaskUpdateDto { Uid = nonExistentUid, Order = 10 } };

        await service.UpdateTasksAsync(updates, userId, clientId: null);

        _notifierServiceMock.Verify(x => x.TaskUpdated(It.IsAny<TasksUpdatedDto>()), Times.Never);
    }
}
