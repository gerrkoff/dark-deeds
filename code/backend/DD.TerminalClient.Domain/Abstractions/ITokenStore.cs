namespace DD.TerminalClient.Domain.Abstractions;

// Stores the profile's JWT separately from the rest of the local state, in its own file tightened to
// owner-only permissions. The token is opaque here: it is never parsed, logged, or combined into the
// state snapshot, so a leaked state file never carries credentials.
public interface ITokenStore
{
    string? Load();

    void Save(string token);

    void Clear();
}
