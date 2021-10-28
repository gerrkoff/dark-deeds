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
        
            Assert.Collection(service.Apply(collection.AsQueryable()).ToList(), _ => { });
        }
    }
}