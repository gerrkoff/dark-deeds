using System.Text.Json;
using System.Text.Json.Serialization;
using DD.Shared.Details.Abstractions.Dto;
using ModelContextProtocol.Protocol;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class McpReader
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) },
    };

    internal static TaskDto[] ReadTasks(CallToolResult result)
    {
        var text = ReadText(result);
        return JsonSerializer.Deserialize<TaskDto[]>(text, JsonOptions)
               ?? throw new InvalidOperationException(
                   "MCP tool returned empty task JSON.");
    }

    private static string ReadText(CallToolResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsError == true)
            throw new InvalidOperationException("MCP tool returned an error result.");

        if (result.Content.Count != 1 || result.Content[0] is not TextContentBlock textBlock)
        {
            throw new InvalidOperationException(
                "MCP tool did not return exactly one text content block.");
        }

        if (string.IsNullOrEmpty(textBlock.Text))
            throw new InvalidOperationException("MCP tool returned empty text.");

        return textBlock.Text;
    }
}
