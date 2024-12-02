using System.Diagnostics.CodeAnalysis;
using DD.ServiceTask.Domain.Entities;
using DD.Shared.Details.Abstractions.Dto;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.TaskServiceTests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Tests")]
public partial class TaskServiceTest
{
    private const string ForeignUserId = "foreign user";
    private const string UserId = "userid";
    private const string Uid = "uid";
    private const string TitleNew = "Task New";
    private const string TitleOld = "Task Old";

    [Fact]
    public async Task SaveTasksAsync_ReturnTasksBack()
    {
        var service = CreateService();

        var items = new[] { new TaskDto { Id = 1000 }, new TaskDto { Id = 2000 } };
        var result = (await service.SaveTasksAsync(items, string.Empty)).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SaveTasksAsync_IgnoreForeignTasks()
    {
        var service = CreateService(new TaskEntity { Uid = Uid, UserId = ForeignUserId });

        var items = new[] { new TaskDto { Uid = Uid } };

        var result = (await service.SaveTasksAsync(items, UserId)).ToList();

        Assert.Empty(result);
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveTasksAsync_WhenDeletingCallDelete()
    {
        var service = CreateService(new TaskEntity { Uid = Uid, UserId = UserId });

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var items = new[] { new TaskDto { Uid = Uid, Deleted = true } };
        var result = await service.SaveTasksAsync(items, UserId);

        Assert.Collection(result, x =>
        {
            Assert.Equal(Uid, x.Uid);
            Assert.True(x.Deleted);
        });
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.Verify(x => x.TryUpdateVersionAsync(It.Is<TaskEntity>(y => y.Uid == Uid && y.IsDeleted)));
        _repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveTasksAsync_IgnoreAlreadyDeletedOnDelete()
    {
        var service = CreateService();

        var items = new[] { new TaskDto { Uid = Uid, Deleted = true } };
        var result = await service.SaveTasksAsync(items, UserId);

        Assert.Collection(result, _ => { });
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveTasksAsync_WhenUpdatingCallTryUpdateVersion()
    {
        var service = CreateService(new TaskEntity { UserId = UserId, Title = TitleOld, Version = 100500, Uid = Uid });

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((true, null)));

        var items = new[] { new TaskDto { Uid = Uid, Title = TitleNew, Version = 100500 } };
        var result = await service.SaveTasksAsync(items, UserId);

        Assert.Collection(result, x =>
        {
            Assert.Equal(Uid, x.Uid);
            Assert.Equal(TitleNew, x.Title);
        });
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.Verify(x => x.TryUpdateVersionAsync(
            It.Is<TaskEntity>(y =>
                y.Uid == Uid &&
                y.Title == TitleNew &&
                y.Version == 100500)));
        _repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveTasksAsync_ReturnNullIfVersionMismatch()
    {
        var service = CreateService(new TaskEntity { Uid = Uid, UserId = UserId, Version = 10, Title = TitleOld });

        _repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<TaskEntity>()))
            .Returns(Task.FromResult<(bool, TaskEntity?)>((false, null)));

        var items = new[] { new TaskDto { Uid = Uid, Version = 9, Title = TitleNew } };
        var result = (await service.SaveTasksAsync(items, UserId)).ToList();

        Assert.Empty(result);
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.Verify(x => x.TryUpdateVersionAsync(
            It.Is<TaskEntity>(y =>
                y.Uid == Uid &&
                y.Title == TitleNew &&
                y.Version == 9)));
        _repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveTasksAsync_WhenCreatingCallUpsert()
    {
        var service = CreateService();

        var items = new[] { new TaskDto { Uid = Uid, Title = TitleNew } };
        var result = (await service.SaveTasksAsync(items, UserId)).ToList();

        Assert.Collection(result, x =>
        {
            Assert.Equal(TitleNew, x.Title);
            Assert.Equal(Uid, x.Uid);
            Assert.Equal(1, x.Version);
        });
        _repoMock.Verify(x => x.GetByIdAsync(Uid));
        _repoMock.Verify(x => x.UpsertAsync(
            It.Is<TaskEntity>(y =>
                y.Uid == Uid &&
                y.UserId == UserId &&
                y.Title == TitleNew &&
                y.Version == 1)));
        _repoMock.VerifyNoOtherCalls();
    }
}
