using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Entities.Models.Abstractions;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests.Mocks
{
    public static partial class MocksCreator
    {
        public static Mock<ITaskRepository> RepoTask(params TaskEntity[] values) => Repo<ITaskRepository, TaskEntity>(values);

        public static Mock<IPlannedRecurrenceRepository> RepoRecurrence(params PlannedRecurrenceEntity[] values) => Repo<IPlannedRecurrenceRepository, PlannedRecurrenceEntity>(values);

        private static Mock<TRepository> Repo<TRepository, TEntity>(params TEntity[] values)
            where TEntity: Entity
            where TRepository: class, IRepository<TEntity> 
        {
            var repoMock = new Mock<TRepository>();
            var result = Task.FromResult(values.ToList() as IList<TEntity>);
            repoMock.Setup(x => x.GetListAsync()).Returns(result);
            repoMock.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<TEntity>>())).Returns(result);
            repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string id) =>
            {
                return Task.FromResult(values.FirstOrDefault(x => x.Uid == id));
            });
            return repoMock;
        }
    }
}