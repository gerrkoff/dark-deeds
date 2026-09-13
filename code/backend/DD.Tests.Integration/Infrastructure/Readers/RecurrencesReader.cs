using DD.ServiceTask.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class RecurrencesReader
{
    internal static Task<PlannedRecurrenceDto[]> ReadAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<PlannedRecurrenceDto[]>(
            response,
            cancellationToken);
    }

    internal static Task<int> ReadCountAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<int>(
            response,
            cancellationToken);
    }
}
