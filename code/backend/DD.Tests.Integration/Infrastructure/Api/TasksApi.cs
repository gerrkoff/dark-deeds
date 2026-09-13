using System.Globalization;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.Tests.Integration.Infrastructure.Api;

internal static class TasksApi
{
    private static readonly Uri TasksUri = new("api/task/tasks", UriKind.Relative);

    internal static Task<HttpResponseMessage> LoadAsync(
        HttpClient client,
        DateTime from,
        CancellationToken cancellationToken = default)
    {
        var value = from.ToString("O", CultureInfo.InvariantCulture);
        var uri = new Uri(
            $"{TasksUri}?from={Uri.EscapeDataString(value)}",
            UriKind.Relative);
        return client.GetAsync(uri, cancellationToken);
    }

    internal static async Task<HttpResponseMessage> SaveAsync(
        HttpClient client,
        IReadOnlyCollection<TaskDto> tasks,
        string? clientId = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, TasksUri)
        {
            Content = JsonContent.Create(tasks),
        };
        if (!string.IsNullOrWhiteSpace(clientId))
            request.Headers.Add("X-Client-Id", clientId);

        return await client.SendAsync(request, cancellationToken);
    }
}
