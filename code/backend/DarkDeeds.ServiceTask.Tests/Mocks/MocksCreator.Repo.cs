using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Entities;
using DarkDeeds.ServiceTask.Entities.Abstractions;
using DarkDeeds.ServiceTask.Infrastructure.Data;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;
using Moq;

namespace DarkDeeds.ServiceTask.Tests.Mocks
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
            repoMock.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<TEntity>>())).Returns(result);
            repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string id) =>
            {
                return Task.FromResult(values.FirstOrDefault(x => x.Uid == id));
            });
            repoMock.Setup(x =>
                    x.TryUpdateVersionPropsAsync(It.IsAny<TEntity>(), It.IsAny<Expression<Func<TEntity, object>>[]>()))
                .Returns(Task.FromResult<(bool, TEntity)>((true, null)));

            return repoMock;
        }
    }
}
