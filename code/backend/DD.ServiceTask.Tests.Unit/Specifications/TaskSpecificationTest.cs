using System.Diagnostics.CodeAnalysis;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.ServiceTask.Domain.Specifications;
using Xunit;

namespace DD.ServiceTask.Tests.Unit.Specifications;

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

    private List<TaskEntity> Collection()
    {
        return
        [
            new TaskEntity { Uid = "1", Date = new DateTime(2018, 10, 10), IsCompleted = true },
            new TaskEntity { Uid = "2", Date = new DateTime(2018, 10, 11) },
            new TaskEntity { Uid = "11", Date = new DateTime(2018, 10, 19), Type = TaskType.Additional },
            new TaskEntity { Uid = "3", Date = new DateTime(2018, 10, 20) },
            new TaskEntity { Uid = "6", Date = new DateTime(2018, 10, 26) },
            new TaskEntity { Uid = "5", Date = new DateTime(2018, 10, 25) },
            new TaskEntity { Uid = "4" }
        ];
    }
}
