using System.Diagnostics.CodeAnalysis;
using DD.MobileClient.Domain.Services;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Moq;
using Xunit;

namespace DD.Tests.Unit.MobileClient.Services;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Tests")]
public class WatchPayloadControllerTests
{
    [Fact]
    public void FilterTasksForStatus_IgnoresCompletedTask()
    {
        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Completed = true },
            new TaskDto { Title = "Task 2", Completed = false },
        };

        var result = WatchPayloadController.FilterTasksForStatus(tasks).ToList();

        Assert.Single(result);
        Assert.Equal("Task 2", result.First().Title);
    }

    [Fact]
    public void FilterTasksForStatus_IgnoresAdditionalTask()
    {
        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Type = TaskTypeDto.Additional },
            new TaskDto { Title = "Task 2", Type = TaskTypeDto.Simple },
        };

        var result = WatchPayloadController.FilterTasksForStatus(tasks).ToList();

        Assert.Single(result);
        Assert.Equal("Task 2", result.First().Title);
    }

    [Fact]
    public void FilterTasksForStatus_SortsTasks()
    {
        var tasks = new[]
        {
            new TaskDto { Title = "Task 2", Order = 2 },
            new TaskDto { Title = "Task 1", Order = 1 },
        };

        var result = WatchPayloadController.FilterTasksForStatus(tasks).ToList();

        Assert.Collection(
            result,
            task => Assert.Equal("Task 1", task.Title),
            task => Assert.Equal("Task 2", task.Title));
    }

    [Fact]
    public void GetWidgetStatus_OnlySimple()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        var tasks = new[]
        {
            new TaskDto { Title = "Task 1" },
        };

        var result = controller.GetWidgetStatus(tasks);

        Assert.Equal("\ud83d\udccc 1 remaining", result.Header);
        Assert.Equal("Task 1", result.Main);
        Assert.Empty(result.Support);
    }

    [Fact]
    public void GetWidgetStatus_OnlySupport()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Type = TaskTypeDto.Routine },
        };

        var result = controller.GetWidgetStatus(tasks);

        Assert.Equal("🎉 all finished!", result.Header);
        Assert.Empty(result.Main);
        Assert.Equal("Task 1", result.Support);
    }

    [Fact]
    public void GetWidgetStatus_SeveralSimple()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Type = TaskTypeDto.Simple },
            new TaskDto { Title = "Task 2", Type = TaskTypeDto.Simple },
        };

        var result = controller.GetWidgetStatus(tasks);

        Assert.Equal("\ud83d\udccc 2 remaining", result.Header);
        Assert.Equal("Task 1", result.Main);
        Assert.Empty(result.Support);
    }

    [Fact]
    public void GetWidgetStatus_NoTasks()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        IReadOnlyCollection<TaskDto> tasks = [];

        var result = controller.GetWidgetStatus(tasks);

        Assert.Equal("🎉 all finished!", result.Header);
        Assert.Empty(result.Main);
        Assert.Empty(result.Support);
    }

    [Fact]
    public void GetWidgetStatus_SimpleAndRoutine()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Type = TaskTypeDto.Routine },
            new TaskDto { Title = "Task 2", Type = TaskTypeDto.Simple },
        };

        var result = controller.GetWidgetStatus(tasks);

        Assert.Equal("\ud83d\udccc 1 remaining", result.Header);
        Assert.Equal("Task 2", result.Main);
        Assert.Equal("Task 1", result.Support);
    }

    [Fact]
    public void GetWidgetStatus_IgnoreRoutineIfItIsAfterSimple()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Type = TaskTypeDto.Simple },
            new TaskDto { Title = "Task 2", Type = TaskTypeDto.Routine },
        };

        var result = controller.GetWidgetStatus(tasks);

        Assert.Equal("\ud83d\udccc 1 remaining", result.Header);
        Assert.Equal("Task 1", result.Main);
        Assert.Empty(result.Support);
    }

    [Fact]
    public void GetAppStatus_SeveralSimpleAndRoutine()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        var tasks = new[]
        {
            new TaskDto { Title = "Task 1", Type = TaskTypeDto.Routine },
            new TaskDto { Title = "Task 2", Type = TaskTypeDto.Simple },
            new TaskDto { Title = "Task 3", Type = TaskTypeDto.Simple },
            new TaskDto { Title = "Task 4", Type = TaskTypeDto.Routine },
            new TaskDto { Title = "Task 5", Type = TaskTypeDto.Simple },
        };

        var result = controller.GetAppStatus(tasks);

        Assert.Equal("\ud83d\udccc 3 remaining", result.Header);
        Assert.Collection(
            result.Items,
            x =>
            {
                Assert.Equal("Task 1", x.Item);
                Assert.True(x.IsSupport);
            },
            x =>
            {
                Assert.Equal("Task 2", x.Item);
                Assert.False(x.IsSupport);
            },
            x =>
            {
                Assert.Equal("Task 3", x.Item);
                Assert.False(x.IsSupport);
            },
            x =>
            {
                Assert.Equal("Task 4", x.Item);
                Assert.True(x.IsSupport);
            },
            x =>
            {
                Assert.Equal("Task 5", x.Item);
                Assert.False(x.IsSupport);
            });
    }

    [Fact]
    public void GetAppStatus_NoTasks()
    {
        var taskConverter = SetupTaskConverter();

        var controller = new WatchPayloadController(taskConverter);

        IReadOnlyCollection<TaskDto> tasks = [];

        var result = controller.GetAppStatus(tasks);

        Assert.Equal("🎉 all finished!", result.Header);
        Assert.Empty(result.Items);
    }

    private static ITaskPrinter SetupTaskConverter()
    {
        var converter = new Mock<ITaskPrinter>();
        converter
            .Setup(x => x.PrintContent(It.IsAny<TaskDto>()))
            .Returns((TaskDto task) => task.Title);
        return converter.Object;
    }
}
