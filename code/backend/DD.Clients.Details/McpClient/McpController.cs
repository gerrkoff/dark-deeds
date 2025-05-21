using System.ComponentModel.DataAnnotations;
using DD.McpClient.Domain;
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
}
