using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.Tests.Integration.Helpers;

internal static class RecurrencesHelper
{
    public static string CreateUniqueRecurrenceUid()
    {
        return Guid.NewGuid().ToString();
    }

    public static PlannedRecurrenceDto CreateRecurrence(
        string task,
        DateTime startDate,
        DateTime? endDate = null,
        int? everyNthDay = null,
        RecurrenceWeekday? everyWeekday = null,
        string? everyMonthDay = null)
    {
        return new PlannedRecurrenceDto
        {
            Uid = CreateUniqueRecurrenceUid(),
            Task = task,
            StartDate = startDate,
            EndDate = endDate,
            EveryNthDay = everyNthDay,
            EveryWeekday = everyWeekday,
            EveryMonthDay = everyMonthDay,
        };
    }
}
