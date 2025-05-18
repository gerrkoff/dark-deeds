using System.Text.Json;
using System.Text.Json.Serialization;
using DD.Shared.Details.Abstractions;
using Microsoft.Extensions.Logging;

namespace DD.McpClient.Domain;

public interface IMcpService
{
    Task<string> LoadTasksByDateAsync(DateTime from, DateTime till, string userId);
}

public sealed class McpService(
    ITaskServiceApp taskServiceApp,
    ILogger<McpService> logger) : IMcpService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<string> LoadTasksByDateAsync(DateTime from, DateTime till, string userId)
    {
        Log.LoadTasks(logger, from, till);
        var tasks = await taskServiceApp.LoadTasksByDateAsync(from, till, userId);

        return JsonSerializer.Serialize(tasks, JsonOptions);
    }
}
