using System.Linq.Expressions;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Abstractions;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Specifications;
using Moq;

namespace DD.Tests.Unit.ServiceTask.Mocks;

public static partial class MocksCreator
{
    public static Mock<ITaskRepository> RepoTask(params TaskEntity[] values)
    {
        return Repo<ITaskRepository, TaskEntity>(values);
    }

    public static Mock<IPlannedRecurrenceRepository> RepoRecurrence(params PlannedRecurrenceEntity[] values)
    {
        return Repo<IPlannedRecurrenceRepository, PlannedRecurrenceEntity>(values);
    }

    private static Mock<TRepository> Repo<TRepository, TEntity>(params TEntity[] values)
        where TEntity : Entity
        where TRepository : class, IRepository<TEntity>
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
            .Returns(Task.FromResult<(bool, TEntity?)>((true, null)));

        return repoMock;
    }
}
