using DD.Shared.Details.Abstractions.Dto;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class TasksReader
{
    internal static Task<TaskDto[]> ReadAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<TaskDto[]>(
            response,
            "tasks",
            cancellationToken);
    }
}
