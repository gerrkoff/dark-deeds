using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Specifications;

public interface ITaskSpecification : IEntitySpecification<TaskEntity, ITaskSpecification>,
    IUserOwnedSpecification<TaskEntity, ITaskSpecification>
{
    ITaskSpecification FilterActual(DateTime from);
    ITaskSpecification FilterDateInterval(DateTime from, DateTime to);
}

public class TaskSpecification : UserOwnedSpecification<TaskEntity, ITaskSpecification>, ITaskSpecification
{
    public ITaskSpecification FilterActual(DateTime from)
    {
        Filters.Add(x => !x.IsCompleted && x.Type != TaskTypeEnum.Additional ||
                         !x.Date.HasValue ||
                         x.Date >= from);
        return this;
    }

    public ITaskSpecification FilterDateInterval(DateTime from, DateTime to)
    {
        Filters.Add(x => x.Date.HasValue && x.Date >= from && x.Date < to);
        return this;
    }
}
