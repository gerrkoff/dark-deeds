using System.Collections.Concurrent;

namespace DD.ServiceTask.Details.Web.Hubs;

public interface IHubClientConnectionTracker
{
    void AddConnection(string connectionId, string? clientId);

    void RemoveConnection(string connectionId);

    IReadOnlyList<string> GetConnectionIdsByClientId(string clientId);
}

internal sealed class HubClientConnectionTracker : IHubClientConnectionTracker
{
    private readonly ConcurrentDictionary<string, string> _connectionToClient = new();

    public void AddConnection(string connectionId, string? clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return;

        _connectionToClient[connectionId] = clientId;
    }

    public void RemoveConnection(string connectionId)
    {
        _connectionToClient.TryRemove(connectionId, out _);
    }

    public IReadOnlyList<string> GetConnectionIdsByClientId(string clientId)
    {
        var connections = new List<string>();

        foreach (var kvp in _connectionToClient)
        {
            if (kvp.Value == clientId)
            {
                connections.Add(kvp.Key);
            }
        }

        return connections;
    }
}
