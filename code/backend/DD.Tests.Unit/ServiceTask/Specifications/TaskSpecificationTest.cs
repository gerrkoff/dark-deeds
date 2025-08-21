using System.Diagnostics.CodeAnalysis;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.ServiceTask.Domain.Specifications;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Specifications;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class TaskSpecificationTest
{
    [Fact]
    public void FilterActual_IncludeNoDate()
    {
        var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.Contains(result, x => x.Uid == "4");
    }

    [Fact]
    public void FilterActual_IncludeExpiredButCompleted()
    {
        var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.Contains(result, x => x.Uid == "2");
    }

    [Fact]
    public void FilterActual_ExcludeExpiredAndCompleted()
    {
        var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.DoesNotContain(result, x => x.Uid == "1");
    }

    [Fact]
    public void FilterActual_ExcludeExpiredAdditional()
    {
        var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.DoesNotContain(result, x => x.Uid == "11");
    }

    [Fact]
    public void FilterActual_ExcludeExpiredRoutine()
    {
        var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.DoesNotContain(result, x => x.Uid == "12");
    }

    [Fact]
    public void FilterDateInterval_ExcludeNoDate()
    {
        var service = new TaskSpecification().FilterDateInterval(
            new DateTime(2018, 10, 20),
            new DateTime(2018, 10, 26));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.DoesNotContain(result, x => x.Uid == "4");
    }

    [Fact]
    public void FilterDateInterval_IncludeOnlyTasksFromPeriod()
    {
        var service = new TaskSpecification().FilterDateInterval(
            new DateTime(2018, 10, 20),
            new DateTime(2018, 10, 26));

        var result = service.Apply(Collection().AsQueryable()).ToList();

        Assert.Contains(result, x => x.Uid == "3"); // FROM border is included
        Assert.Contains(result, x => x.Uid == "5");
        Assert.DoesNotContain(result, x => x.Uid == "1");
        Assert.DoesNotContain(result, x => x.Uid == "2");
        Assert.DoesNotContain(result, x => x.Uid == "6"); // TO border is not included
    }

    [Fact]
    public void FilterActual_IncludeWeeklyInsideWeek()
    {
        var from = new DateTime(2018, 10, 20);
        var service = new TaskSpecification().FilterActual(from);

        var result = service.Apply(CollectionWithWeekly().AsQueryable()).ToList();

        Assert.Contains(result, x => x.Uid == "w1");
    }

    [Fact]
    public void FilterActual_ExcludeWeeklyOutsideWeek()
    {
        var from = new DateTime(2018, 10, 20);
        var service = new TaskSpecification().FilterActual(from);

        var result = service.Apply(CollectionWithWeekly().AsQueryable()).ToList();

        Assert.DoesNotContain(result, x => x.Uid == "w2");
    }

    [Fact]
    public void FilterActual_PassThroughNonWeekly()
    {
        var from = new DateTime(2018, 10, 20);
        var service = new TaskSpecification().FilterActual(from);

        var result = service.Apply(CollectionWithWeekly().AsQueryable()).ToList();

        Assert.Contains(result, x => x.Uid == "normal");
    }

    private static List<TaskEntity> Collection()
    {
        return
        [
            new TaskEntity { Uid = "1", Date = new DateTime(2018, 10, 10), IsCompleted = true },
            new TaskEntity { Uid = "2", Date = new DateTime(2018, 10, 11) },
            new TaskEntity { Uid = "11", Date = new DateTime(2018, 10, 19), Type = TaskType.Additional },
            new TaskEntity { Uid = "12", Date = new DateTime(2018, 10, 19), Type = TaskType.Routine },
            new TaskEntity { Uid = "3", Date = new DateTime(2018, 10, 20) },
            new TaskEntity { Uid = "6", Date = new DateTime(2018, 10, 26) },
            new TaskEntity { Uid = "5", Date = new DateTime(2018, 10, 25) },
            new TaskEntity { Uid = "4" }
        ];
    }

    private static List<TaskEntity> CollectionWithWeekly()
    {
        return
        [
            new TaskEntity { Uid = "w1", Date = new DateTime(2018, 10, 22), Type = TaskType.Weekly },
            new TaskEntity { Uid = "w2", Date = new DateTime(2018, 10, 29), Type = TaskType.Weekly },
            new TaskEntity { Uid = "normal", Date = new DateTime(2018, 10, 23) }
        ];
    }
}
