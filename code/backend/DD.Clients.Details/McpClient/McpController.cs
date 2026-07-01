using System.ComponentModel.DataAnnotations;
using DD.McpClient.Domain;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.Clients.Details.McpClient;

[AllowAnonymous]
public class McpController(IMcpService mcpService) : ControllerBase
{
    [HttpGet]
    public Task<string> LoadTasks(
        [Required] string userId,
        [Required] DateTime from,
        [Required] DateTime till)
    {
        return mcpService.LoadTasksByDateAsync(from, till, userId);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTasksOrder(
        [Required] string userId,
        [Required] string justification,
        [FromBody] ICollection<TaskUpdateDto> updates)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("userId must be provided.");
        }

        if (string.IsNullOrWhiteSpace(justification))
        {
            return BadRequest("justification must be provided.");
        }

        var result = await mcpService.UpdateTasksOrderAsync(updates, userId, justification);
        return Content(result);
    }
}
