using System.ComponentModel;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using ModelContextProtocol.Server;

namespace DD.Clients.Details.McpClient.Tools;

[McpServerToolType]
internal sealed class UpdateTasksOrderTool
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
        if (string.IsNullOrWhiteSpace(justification))
        {
            throw new ArgumentException("Justification must be provided.", nameof(justification));
        }

        if (updates is null || updates.Count == 0 || updates.Any(update => update is null || string.IsNullOrWhiteSpace(update.Uid)))
        {
            throw new ArgumentException("At least one task update with a non-empty UID must be provided.", nameof(updates));
        }

        var userId = userAuth.UserId();
        return mcpService.UpdateTasksOrderAsync(updates, userId, justification);
    }
}
