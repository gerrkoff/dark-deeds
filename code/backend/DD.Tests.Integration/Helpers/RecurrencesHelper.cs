using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;

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

    public static async Task<int> CreateRecurrencesAsync(HttpClient httpClient, int timezoneOffset = 0)
    {
        using var response = await RecurrencesApi.CreateTasksAsync(
            httpClient,
            timezoneOffset);
        return await RecurrencesReader.ReadCountAsync(response);
    }
}
