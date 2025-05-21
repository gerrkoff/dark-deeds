using System.ComponentModel;
using DarkDeeds.Mcp.Helpers;
using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Mcp.Tools;

[McpServerToolType]
public static class LoadTasksTool
{
    [McpServerTool(Name = "LoadTasks")]
    [Description("Loads tasks (in JSON) from Dark Deeds App for a date range.")]
    public static Task<string> Do(
        IMcpServer server,
        [Description("Date from which tasks should be loaded")] DateTime from,
        [Description("Date till which tasks should loaded (not inclusive)")] DateTime till)
    {
        var logger = server.AsClientLoggerProvider().CreateLogger(nameof(LoadTasksTool));

        try
        {
            var httpClient = server.Services!
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient();
            var apiUrl = EnvHelper.GetApiUrl();
            var userId = EnvHelper.GetUserId();
            var apiKey = EnvHelper.GetApiKey();

            var url = $"{apiUrl}/{apiKey}/LoadTasks?userId={userId}&from={from:yyyy-MM-dd}&till={till:yyyy-MM-dd}";

            return httpClient.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            logger.LogInformation("{Service} exception: {Exception})", nameof(LoadTasksTool), ex.Message);
            Console.Write($"{nameof(LoadTasksTool)} exception: {ex.Message}");

            throw;
        }
    }
}
