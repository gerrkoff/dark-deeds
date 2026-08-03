namespace DD.TerminalClient.Details.Storage;

// Resolves the on-disk locations the terminal client uses. By default all persistent data lives in
// a portable data directory beside the executable. An explicit root override keeps separate config
// and state trees so tests and the unattended self-test can isolate all state in a throwaway
// location. Every profile gets its own directory so tokens, cache, outbox, settings, and logs never
// leak between profiles.
public sealed class ApplicationPathProvider
{
    private const string DataFolderName = "data";
    private const string ProfilesFolderName = "profiles";

    private static readonly char[] PathSeparators = ['/', '\\'];

    public ApplicationPathProvider(string? rootOverride = null)
    {
        if (!string.IsNullOrWhiteSpace(rootOverride))
        {
            var root = Path.GetFullPath(rootOverride);
            ConfigRoot = Path.Combine(root, "config");
            StateRoot = Path.Combine(root, "state");
        }
        else
        {
            var dataRoot = Path.Combine(AppContext.BaseDirectory, DataFolderName);
            ConfigRoot = dataRoot;
            StateRoot = dataRoot;
        }
    }

    public string ConfigRoot { get; }

    public string StateRoot { get; }

    public string GetProfileConfigDirectory(string profileName)
    {
        return Path.Combine(ConfigRoot, ProfilesFolderName, RequireSingleSegment(profileName));
    }

    public string GetProfileStateDirectory(string profileName)
    {
        return Path.Combine(StateRoot, ProfilesFolderName, RequireSingleSegment(profileName));
    }

    // Defense in depth: even though callers pass names already validated by TerminalProfile, refuse
    // to build a path from anything that is not a single, non-traversing path segment.
    private static string RequireSingleSegment(string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName) ||
            profileName.IndexOfAny(PathSeparators) >= 0 ||
            profileName.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Profile name '{profileName}' is not a single safe path segment.", nameof(profileName));
        }

        return profileName;
    }
}
