using DarkDeeds.ServiceTask.Entities;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    public interface IPlannedRecurrenceSpecification : IEntitySpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>,
        IUserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>
    {
    }
}
