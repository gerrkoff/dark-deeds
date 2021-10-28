using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests.Mocks
{
    public static partial class MocksCreator
    {
        public static Mock<ITaskRepository> RepoTask(params TaskEntity[] values)
        {
            var repoMock = new Mock<ITaskRepository>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            var result = Task.FromResult(values.ToList() as IList<TaskEntity>);
            repoMock.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<TaskEntity>>())).Returns(result);
            repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string id) =>
            {
                return Task.FromResult(values.FirstOrDefault(x => x.Uid == id));
            });
            return repoMock;
        }
        
        public static Mock<IPlannedRecurrenceRepository> RepoRecurrence(params PlannedRecurrenceEntity[] values)
        {
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            var result = Task.FromResult(values.ToList() as IList<PlannedRecurrenceEntity>);
            repoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns((int id) =>
            {
                return Task.FromResult(values.FirstOrDefault(x => x.Id == id));
            });
            return repoMock;
        }
    }
}