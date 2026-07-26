using System.Globalization;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Time;

namespace DD.TerminalClient.Details.Api;

// The bearer-authenticated task transport. Load always requests the full snapshot from the current
// local Monday mapped to UTC midnight - the start of the visible period - so a timezone change never
// moves the window. Save posts the batch as the shared contract and returns the server's authoritative
// copies mapped back to terminal tasks, letting the caller apply new versions without a hub echo.
internal sealed class TaskApiClient(HttpClient httpClient, ILocalDateProvider localDateProvider)
    : TerminalHttpClientBase(httpClient), ITaskApiClient
{
    private const string TasksPath = "api/task/tasks";

    private readonly ILocalDateProvider _localDateProvider =
        localDateProvider ?? throw new ArgumentNullException(nameof(localDateProvider));

    public async Task<IReadOnlyList<TerminalTask>> LoadTasksAsync(CancellationToken cancellationToken)
    {
        var monday = CurrentLocalMonday(_localDateProvider.Today);
        var from = TaskTransportMapper.ToTransportInstant(monday)
            .ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        var requestUri = $"{TasksPath}?from={Uri.EscapeDataString(from)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await SendAsync(request, cancellationToken);
        EnsureSuccess(response);
        var dtos = await ReadJsonAsync<List<TaskDto>>(response, cancellationToken);

        return dtos.Select(TaskTransportMapper.ToTerminalTask).ToList();
    }

    public async Task<IReadOnlyList<TerminalTask>> SaveTasksAsync(
        IReadOnlyCollection<TerminalTask> tasks, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        var payload = tasks.Select(TaskTransportMapper.ToDto).ToList();

        using var request = new HttpRequestMessage(HttpMethod.Post, TasksPath)
        {
            Content = JsonContent.Create(payload, options: JsonOptions),
        };

        using var response = await SendAsync(request, cancellationToken);
        EnsureSuccess(response);
        var dtos = await ReadJsonAsync<List<TaskDto>>(response, cancellationToken);

        return dtos.Select(TaskTransportMapper.ToTerminalTask).ToList();
    }

    // The Monday on or before the given day, where the week starts on Monday (DayOfWeek has Sunday = 0).
    private static DateOnly CurrentLocalMonday(DateOnly today)
    {
        var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
        return today.AddDays(-daysSinceMonday);
    }
}
