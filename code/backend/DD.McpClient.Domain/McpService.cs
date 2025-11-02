using System.Text.Json;
using System.Text.Json.Serialization;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;

namespace DD.McpClient.Domain;

public interface IMcpService
{
    Task<string> LoadTasksByDateAsync(DateTime from, DateTime till, string userId);

    Task<string> UpdateTasksOrderAsync(ICollection<TaskUpdateDto> updates, string userId);
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

    public async Task<string> UpdateTasksOrderAsync(ICollection<TaskUpdateDto> updates, string userId)
    {
        Log.UpdateTasksOrder(logger, updates.Count);
        var tasks = await taskServiceApp.UpdateTasksAsync(updates, userId, clientId: null);
        return JsonSerializer.Serialize(tasks, JsonOptions);
    }
}
