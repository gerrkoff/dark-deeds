using System.Text.Json;
using System.Text.Json.Serialization;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;

namespace DD.McpClient.Domain;

public interface IMcpService
{
    Task<string> LoadTasksByDateAsync(DateTime from, DateTime till, string userId);

    Task<string> UpdateTasksOrderAsync(ICollection<TaskUpdateDto> updates, string userId, string justification);

    Task<string> AddTasksAsync(ICollection<TaskCreateDto> tasks, string userId, string justification);
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

    public async Task<string> UpdateTasksOrderAsync(ICollection<TaskUpdateDto> updates, string userId, string justification)
    {
        if (string.IsNullOrWhiteSpace(justification))
        {
            throw new ArgumentException("Justification must be provided.", nameof(justification));
        }

        Log.UpdateTasksOrder(logger, updates.Count, justification);
        var tasks = await taskServiceApp.UpdateTasksAsync(updates, userId, clientId: null);
        return JsonSerializer.Serialize(tasks, JsonOptions);
    }

    public async Task<string> AddTasksAsync(ICollection<TaskCreateDto> tasks, string userId, string justification)
    {
        if (string.IsNullOrWhiteSpace(justification))
        {
            throw new ArgumentException("Justification must be provided.", nameof(justification));
        }

        if (tasks is null || tasks.Count == 0 || tasks.Any(task => task is null || string.IsNullOrWhiteSpace(task.Title)))
        {
            throw new ArgumentException("At least one task with a non-empty title must be provided.", nameof(tasks));
        }

        Log.AddTasks(logger, tasks.Count, justification);

        var newTasks = tasks
            .Select(task => new TaskDto
            {
                Uid = Guid.NewGuid().ToString(),
                Title = task.Title,
                Date = task.Date,
                Time = task.Time,
                Type = task.Type,
                IsProbable = task.IsProbable,
            })
            .ToArray();

        var savedTasks = await taskServiceApp.SaveTasksAsync(newTasks, userId, clientId: null);
        return JsonSerializer.Serialize(savedTasks, JsonOptions);
    }
}
