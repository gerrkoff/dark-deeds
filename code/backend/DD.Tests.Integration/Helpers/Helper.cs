using System.Globalization;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.Tests.Integration.Helpers;

internal static class Helper
{
    public static string CreateUniqueUsername(string prefix = "test")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    public static string CreateUniqueTaskUid()
    {
        return Guid.NewGuid().ToString();
    }

    public static void ReportCleanupFailure(Exception exception)
    {
        Console.Error.WriteLine($"Integration test cleanup failed: {exception}");
    }

    public static async Task<TaskDto[]> ReadTasksAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskDto[]>()
               ?? throw new InvalidOperationException("The task response was empty.");
    }

    public static Uri CreateTasksUri(DateTime from)
    {
        var value = from.ToString("O", CultureInfo.InvariantCulture);
        return new Uri(
            $"api/task/tasks?from={Uri.EscapeDataString(value)}",
            UriKind.Relative);
    }
}
