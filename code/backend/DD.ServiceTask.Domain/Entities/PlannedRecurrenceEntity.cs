using DD.ServiceTask.Domain.Entities.Abstractions;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Entities;

public class PlannedRecurrenceEntity : Entity, IUserOwnedEntity
{
    public string Task { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? EveryNthDay { get; set; }

    public string EveryMonthDay { get; set; } = string.Empty;

    public RecurrenceWeekday? EveryWeekday { get; set; }

    public string UserId { get; set; } = string.Empty;

    public List<RecurrenceEntity> Recurrences { get; set; } = [];
}
