using System.ComponentModel;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using ModelContextProtocol.Server;

namespace DD.Clients.Details.McpClient.Tools;

[McpServerToolType]
internal sealed class AddTasksTool
{
    [McpServerTool(Name = "AddTasks")]
    [Description("Adds new tasks to Dark Deeds App.")]
    public static async Task<string> Do(
        IMcpService mcpService,
        IUserAuth userAuth,
        [Description("Array of new tasks to create")]
        ICollection<TaskCreateDto> tasks,
        [Description("Explain why these tasks should be created. State the concrete evidence and reasoning that justify adding these tasks to the user's list.")]
        string justification)
    {
        if (string.IsNullOrWhiteSpace(justification))
        {
            throw new ArgumentException("Justification must be provided.", nameof(justification));
        }

        if (tasks is null || tasks.Count == 0 || tasks.Any(task => task is null || string.IsNullOrWhiteSpace(task.Title)))
        {
            throw new ArgumentException("At least one task with a non-empty title must be provided.", nameof(tasks));
        }

        var userId = userAuth.UserId();
        return await mcpService.AddTasksAsync(tasks, userId, justification);
    }
}
