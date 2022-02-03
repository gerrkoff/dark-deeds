using DarkDeeds.ServiceTask.Entities.Models;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    public interface IPlannedRecurrenceSpecification : IEntitySpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>,
        IUserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>
    {
    }
}