using DD.ServiceTask.Domain.Dto;

namespace DD.ServiceTask.Domain.DtoExtensions;

internal static class RecurrenceExtensions
{
    public static ICollection<PlannedRecurrenceDto> ToUtcDate(this ICollection<PlannedRecurrenceDto> recurrences)
    {
        foreach (var recurrence in recurrences)
        {
            recurrence.StartDate = DateTime.SpecifyKind(recurrence.StartDate, DateTimeKind.Utc);
            if (recurrence.EndDate != null)
                recurrence.EndDate = DateTime.SpecifyKind(recurrence.EndDate.Value, DateTimeKind.Utc);
        }

        return recurrences;
    }
}
