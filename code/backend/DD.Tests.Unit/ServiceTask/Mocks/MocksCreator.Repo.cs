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
        var mock = Repo<IPlannedRecurrenceRepository, PlannedRecurrenceEntity>(values);

        // Track added recurrences to simulate the real behavior
        var addedRecurrences = new HashSet<(string Uid, DateTime Date)>();

        // Setup TryAddRecurrenceAsync to check if the date already exists in the entity or was already added
        mock.Setup(x => x.TryAddRecurrenceAsync(It.IsAny<string>(), It.IsAny<RecurrenceEntity>()))
            .ReturnsAsync((string uid, RecurrenceEntity recurrence) =>
            {
                // Check if this date already exists in the original entity
                var entity = values.FirstOrDefault(e => e.Uid == uid);
                if (entity?.Recurrences.Any(r => r.DateTime == recurrence.DateTime) == true)
                {
                    return false; // Already exists
                }

                // Check if we already added this date in this test run
                var key = (uid, recurrence.DateTime);
                if (addedRecurrences.Contains(key))
                {
                    return false; // Already added
                }

                // Mark as added
                addedRecurrences.Add(key);
                return true;
            });

        // Setup TryUpdateRecurrenceTaskUidAsync to always return true (successfully updated)
        mock.Setup(x => x.TryUpdateRecurrenceTaskUidAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        return mock;
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
