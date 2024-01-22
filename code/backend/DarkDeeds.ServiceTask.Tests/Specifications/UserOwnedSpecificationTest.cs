using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DD.TaskService.Domain.Entities;
using DD.TaskService.Domain.Specifications;
using Xunit;

namespace DarkDeeds.ServiceTask.Tests.Specifications;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class UserOwnedSpecificationTest
{
    [Fact]
    public void FilterUserOwned_Positive()
    {
        var collection = new List<TaskEntity>
        {
            new() {UserId = "userid1"},
            new() {UserId = "userid2"}
        };

        var service = new TaskSpecification().FilterUserOwned("userid1");

        var result = service.Apply(collection.AsQueryable()).ToList();

        Assert.Collection(result, _ => { });
    }

    [Fact]
    public void FilterForeignUserOwned_Positive()
    {
        var collection = new List<TaskEntity>
        {
            new() {UserId = "userid1", Uid = "uid1"},
            new() {UserId = "userid2", Uid = "uid2"}
        };

        var service = new TaskSpecification().FilterForeignUserOwned("userid1", new[] {"uid1", "uid2"});

        var result = service.Apply(collection.AsQueryable()).ToList();

        Assert.Collection(result, x =>
        {
            Assert.Equal("uid2", x.Uid);
        });
    }
}
