using System.ComponentModel;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using ModelContextProtocol.Server;

namespace DD.Clients.Details.McpClient.Tools;

[McpServerToolType]
public sealed class UpdateTasksOrderTool
{
    [McpServerTool(Name = "UpdateTasksOrder")]
    [Description("Updates the order of multiple tasks in Dark Deeds App.")]
    public static Task<string> Do(
        IMcpService mcpService,
        IUserAuth userAuth,
        [Description("Array of task updates with UID and new order")]
        ICollection<TaskUpdateDto> updates,
        [Description("Explain why this new ordering is correct. State the concrete evidence and reasoning that justify reordering the user's tasks this way.")]
        string justification)
    {
        var userId = userAuth.UserId();
        return mcpService.UpdateTasksOrderAsync(updates, userId, justification);
    }
}
