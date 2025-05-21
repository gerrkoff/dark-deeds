using Microsoft.Extensions.DependencyInjection;

namespace DD.McpClient.Domain;

public static class Setup
{
    public static void AddMcpClientDomain(this IServiceCollection services)
    {
        services.AddScoped<IMcpService, McpService>();
    }
}
