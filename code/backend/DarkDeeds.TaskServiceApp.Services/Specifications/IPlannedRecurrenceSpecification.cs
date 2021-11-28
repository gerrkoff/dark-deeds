using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public interface IPlannedRecurrenceSpecification : IEntitySpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>,
        IUserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>
    {
    }
}