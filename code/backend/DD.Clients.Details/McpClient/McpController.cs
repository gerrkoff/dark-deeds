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
    public Task<string> UpdateTasksOrder(
        [Required] string userId,
        [FromBody] ICollection<TaskUpdateDto> updates)
    {
        return mcpService.UpdateTasksOrderAsync(updates, userId);
    }
}
