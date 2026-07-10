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
    public static Task<string> Do(
        IMcpService mcpService,
        IUserAuth userAuth,
        [Description("Array of new tasks to create")]
        ICollection<TaskCreateDto> tasks,
        [Description("Explain why these tasks should be created. State the concrete evidence and reasoning that justify adding these tasks to the user's list.")]
        string justification)
    {
        var userId = userAuth.UserId();
        return mcpService.AddTasksAsync(tasks, userId, justification);
    }
}
