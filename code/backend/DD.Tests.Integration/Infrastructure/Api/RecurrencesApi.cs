using System.Net.Http.Json;
using DD.ServiceTask.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Api;

internal static class RecurrencesApi
{
    private static readonly Uri RecurrencesUri =
        new("api/task/recurrences", UriKind.Relative);

    internal static Task<HttpResponseMessage> LoadAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(RecurrencesUri, cancellationToken);
    }

    internal static Task<HttpResponseMessage> SaveAsync(
        HttpClient client,
        IReadOnlyCollection<PlannedRecurrenceDto> recurrences,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(RecurrencesUri, recurrences, cancellationToken);
    }

    internal static Task<HttpResponseMessage> CreateTasksAsync(
        HttpClient client,
        int timezoneOffset = 0,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(
            $"{RecurrencesUri}/create?timezoneOffset={timezoneOffset}",
            UriKind.Relative);
        return client.PostAsync(uri, content: null, cancellationToken);
    }
}
