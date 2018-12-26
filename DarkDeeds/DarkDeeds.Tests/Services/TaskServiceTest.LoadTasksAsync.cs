using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models;
using DarkDeeds.Services.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class TaskServiceTest
    {
        [Fact]
        public async Task ReturnOnlyUserTask()
        {
            var repo = Helper.CreateRepoMock(
                new TaskEntity {UserId = "1", Id = 1000},
                new TaskEntity {UserId = "2", Id = 2000}
            ).Object;
            
            var service = new TaskService(repo);

            var result = (await service.LoadTasksAsync(new CurrentUser {UserId = "1"})).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(1000, result[0].Id);
        }
    }
}