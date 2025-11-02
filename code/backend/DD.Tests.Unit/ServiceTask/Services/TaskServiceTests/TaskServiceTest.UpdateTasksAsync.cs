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
    public async Task UpdateTasksAsync_SuccessfulUpdate_ReturnsUpdatedTasks()
    {
        var uid1 = "uid1";
        var uid2 = "uid2";
        var userId = "user123";

        var entity1 = new TaskEntity { Uid = uid1, UserId = userId, Order = 1, Version = 1 };
        var entity2 = new TaskEntity { Uid = uid2, UserId = userId, Order = 2, Version = 1 };
        var service = CreateService(entity1, entity2);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = uid1, Order = 10 },
            new TaskUpdateDto { Uid = uid2, Order = 20 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Uid == uid1 && x.Order == 10);
        Assert.Contains(result, x => x.Uid == uid2 && x.Order == 20);

        _repoMock.Verify(x => x.GetByIdAsync(uid1), Times.Once);
        _repoMock.Verify(x => x.GetByIdAsync(uid2), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.Is<TaskEntity>(e => e.Uid == uid1 && e.Order == 10)), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.Is<TaskEntity>(e => e.Uid == uid2 && e.Order == 20)), Times.Once);
        _notifierServiceMock.Verify(
            x => x.TaskUpdated(
                It.Is<TasksUpdatedDto>(dto =>
                    dto.Tasks.Count() == 2 && dto.UserId == userId)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTasksAsync_NonExistentTask_SkipsAndContinues()
    {
        var existingUid = "existing";
        var nonExistentUid = "nonexistent";
        var userId = "user123";

        var entity = new TaskEntity { Uid = existingUid, UserId = userId, Order = 1, Version = 1 };
        var service = CreateService(entity);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = nonExistentUid, Order = 10 },
            new TaskUpdateDto { Uid = existingUid, Order = 20 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Single(result);
        Assert.Equal(existingUid, result[0].Uid);
        Assert.Equal(20, result[0].Order);

        _repoMock.Verify(x => x.GetByIdAsync(nonExistentUid), Times.Once);
        _repoMock.Verify(x => x.GetByIdAsync(existingUid), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTasksAsync_DeletedTask_SkipsAndContinues()
    {
        var deletedUid = "deleted";
        var activeUid = "active";
        var userId = "user123";

        var deletedEntity = new TaskEntity
        {
            Uid = deletedUid,
            UserId = userId,
            Order = 1,
            Version = 1,
            DeletedAt = DateTimeOffset.UtcNow,
        };
        var activeEntity = new TaskEntity { Uid = activeUid, UserId = userId, Order = 2, Version = 1 };
        var service = CreateService(deletedEntity, activeEntity);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = deletedUid, Order = 10 },
            new TaskUpdateDto { Uid = activeUid, Order = 20 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Single(result);
        Assert.Equal(activeUid, result[0].Uid);
        Assert.Equal(20, result[0].Order);

        _repoMock.Verify(x => x.GetByIdAsync(deletedUid), Times.Once);
        _repoMock.Verify(x => x.GetByIdAsync(activeUid), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTasksAsync_ForeignTask_SkipsAndContinues()
    {
        var foreignUid = "foreign";
        var ownUid = "own";
        var userId = "user123";
        var foreignUserId = "foreignUser";

        var foreignEntity = new TaskEntity { Uid = foreignUid, UserId = foreignUserId, Order = 1, Version = 1 };
        var ownEntity = new TaskEntity { Uid = ownUid, UserId = userId, Order = 2, Version = 1 };
        var service = CreateService(foreignEntity, ownEntity);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = foreignUid, Order = 10 },
            new TaskUpdateDto { Uid = ownUid, Order = 20 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Single(result);
        Assert.Equal(ownUid, result[0].Uid);
        Assert.Equal(20, result[0].Order);

        _repoMock.Verify(x => x.GetByIdAsync(foreignUid), Times.Once);
        _repoMock.Verify(x => x.GetByIdAsync(ownUid), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTasksAsync_VersionConflict_SkipsAndContinues()
    {
        var conflictUid = "conflict";
        var successUid = "success";
        var userId = "user123";

        var conflictEntity = new TaskEntity { Uid = conflictUid, UserId = userId, Order = 1, Version = 1 };
        var successEntity = new TaskEntity { Uid = successUid, UserId = userId, Order = 2, Version = 1 };
        var service = CreateService(conflictEntity, successEntity);

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.Is<TaskEntity>(e => e.Uid == conflictUid)))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((false, null)));
        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.Is<TaskEntity>(e => e.Uid == successUid)))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = conflictUid, Order = 10 },
            new TaskUpdateDto { Uid = successUid, Order = 20 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Single(result);
        Assert.Equal(successUid, result[0].Uid);
        Assert.Equal(20, result[0].Order);

        _repoMock.Verify(x => x.GetByIdAsync(conflictUid), Times.Once);
        _repoMock.Verify(x => x.GetByIdAsync(successUid), Times.Once);
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()), Times.Exactly(2));
    }

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

    [Fact]
    public async Task UpdateTasksAsync_MultipleUpdatesWithMixedResults()
    {
        var successUid1 = "success1";
        var successUid2 = "success2";
        var nonExistentUid = "nonexistent";
        var deletedUid = "deleted";
        var foreignUid = "foreign";
        var conflictUid = "conflict";
        var userId = "user123";
        var foreignUserId = "foreignUser";

        var successEntity1 = new TaskEntity { Uid = successUid1, UserId = userId, Order = 1, Version = 1 };
        var successEntity2 = new TaskEntity { Uid = successUid2, UserId = userId, Order = 2, Version = 1 };
        var deletedEntity = new TaskEntity
        {
            Uid = deletedUid,
            UserId = userId,
            Order = 3,
            Version = 1,
            DeletedAt = DateTimeOffset.UtcNow,
        };
        var foreignEntity = new TaskEntity { Uid = foreignUid, UserId = foreignUserId, Order = 4, Version = 1 };
        var conflictEntity = new TaskEntity { Uid = conflictUid, UserId = userId, Order = 5, Version = 1 };

        var service = CreateService(successEntity1, successEntity2, deletedEntity, foreignEntity, conflictEntity);

        _repoMock.Setup(
                x => x.TryUpdateVersionAsync(
                    It.Is<TaskEntity>(e =>
                        e.Uid == successUid1 || e.Uid == successUid2)))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));
        _repoMock.Setup(
                x => x.TryUpdateVersionAsync(
                    It.Is<TaskEntity>(e => e.Uid == conflictUid)))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((false, null)));

        var updates = new[]
        {
            new TaskUpdateDto { Uid = nonExistentUid, Order = 10 },
            new TaskUpdateDto { Uid = successUid1, Order = 20 },
            new TaskUpdateDto { Uid = deletedUid, Order = 30 },
            new TaskUpdateDto { Uid = foreignUid, Order = 40 },
            new TaskUpdateDto { Uid = conflictUid, Order = 50 },
            new TaskUpdateDto { Uid = successUid2, Order = 60 },
        };

        var result = (await service.UpdateTasksAsync(updates, userId, clientId: null)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Uid == successUid1 && x.Order == 20);
        Assert.Contains(result, x => x.Uid == successUid2 && x.Order == 60);

        _notifierServiceMock.Verify(
            x => x.TaskUpdated(
                It.Is<TasksUpdatedDto>(dto =>
                    dto.UserId == userId &&
                    dto.Tasks.Count() == 2)),
            Times.Once);
    }
}
