using System.Collections.Generic;
using System.Linq;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Services.Specifications;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Specifications
{
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
}