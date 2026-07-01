using System.ComponentModel;
using System.Net.Http.Json;
using DarkDeeds.Mcp.Helpers;
using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Mcp.Tools;

[McpServerToolType]
public static class UpdateTasksOrderTool
{
    [McpServerTool(Name = "UpdateTasksOrder")]
    [Description("Updates the order of multiple tasks in Dark Deeds App.")]
    public static async Task<string> Do(
        IMcpServer server,
        [Description("Array of task updates with UID and new order")]
        TaskUpdateInput[] updates,
        [Description("Explain why this new ordering is correct. State the concrete evidence and reasoning that justify reordering the user's tasks this way.")]
        string justification)
    {
        var logger = server.AsClientLoggerProvider().CreateLogger(nameof(UpdateTasksOrderTool));

        try
        {
            var httpClient = server.Services!
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient();
            var apiUrl = EnvHelper.GetApiUrl();
            var userId = EnvHelper.GetUserId();
            var apiKey = EnvHelper.GetApiKey();

            var url = $"{apiUrl}/{apiKey}/UpdateTasksOrder?userId={userId}&justification={Uri.EscapeDataString(justification)}";

            var response = await httpClient.PostAsJsonAsync(url, updates);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            logger.LogInformation("{Service} exception: {Exception}", nameof(UpdateTasksOrderTool), ex.Message);
            Console.Write($"{nameof(UpdateTasksOrderTool)} exception: {ex.Message}");

            throw;
        }
    }
}

public class TaskUpdateInput
{
    [Description("Task unique identifier")]
    public string Uid { get; set; } = string.Empty;

    [Description("New order value for the task")]
    public int Order { get; set; }
}
