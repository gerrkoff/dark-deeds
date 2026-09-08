using System.Net.Http.Json;
using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.Tests.Integration.Helpers;

internal static class RecurrencesHelper
{
    public const string RecurrencesRoute = "api/task/recurrences";

    public static readonly Uri RecurrencesUri = new(RecurrencesRoute, UriKind.Relative);

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

    public static async Task<PlannedRecurrenceDto[]> ReadRecurrencesAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlannedRecurrenceDto[]>()
               ?? throw new InvalidOperationException("The recurrence response was empty.");
    }

    public static async Task<int> ReadRecurrenceCountAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public static async Task<int> CreateRecurrencesAsync(HttpClient httpClient, int timezoneOffset = 0)
    {
        using var response = await httpClient.PostAsync(
            new Uri($"{RecurrencesRoute}/create?timezoneOffset={timezoneOffset}", UriKind.Relative),
            content: null);
        return await ReadRecurrenceCountAsync(response);
    }
}
