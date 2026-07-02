using System.ComponentModel;
using DD.McpClient.Domain;
using DD.Shared.Details.Services;
using ModelContextProtocol.Server;

namespace DD.Clients.Details.McpClient.Tools;

[McpServerToolType]
internal sealed class LoadTasksTool
{
    [McpServerTool(Name = "LoadTasks")]
    [Description("Loads tasks (in JSON) from Dark Deeds App for a date range.")]
    public static Task<string> Do(
        IMcpService mcpService,
        IUserAuth userAuth,
        [Description("Date from which tasks should be loaded")] DateTime from,
        [Description("Date till which tasks should be loaded (not inclusive)")] DateTime till)
    {
        var userId = userAuth.UserId();
        return mcpService.LoadTasksByDateAsync(from, till, userId);
    }
}
