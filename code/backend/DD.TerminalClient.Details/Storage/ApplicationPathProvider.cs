namespace DD.TerminalClient.Details.Storage;

// Resolves the on-disk locations the terminal client uses, following OS conventions: on macOS a
// single Application Support tree, on Linux the XDG config and state base directories. An explicit
// root override collapses both trees under one directory so tests and the unattended self-test can
// isolate all state in a throwaway location. Every profile gets its own config and state
// subdirectory so tokens, cache, outbox, settings, and logs never leak between profiles.
public sealed class ApplicationPathProvider
{
    private const string ApplicationFolderName = "dark-deeds-terminal";
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
        else if (OperatingSystem.IsMacOS())
        {
            var applicationSupport = Path.Combine(
                GetHomeDirectory(), "Library", "Application Support", ApplicationFolderName);
            ConfigRoot = applicationSupport;
            StateRoot = applicationSupport;
        }
        else
        {
            ConfigRoot = Path.Combine(
                GetXdgBaseDirectory("XDG_CONFIG_HOME", ".config"), ApplicationFolderName);
            StateRoot = Path.Combine(
                GetXdgBaseDirectory("XDG_STATE_HOME", Path.Combine(".local", "state")), ApplicationFolderName);
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

    private static string GetHomeDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(home))
        {
            home = Environment.GetEnvironmentVariable("HOME") ?? string.Empty;
        }

        if (string.IsNullOrEmpty(home))
        {
            throw new InvalidOperationException("Unable to resolve the user home directory.");
        }

        return home;
    }

    private static string GetXdgBaseDirectory(string variable, string relativeFallback)
    {
        // The XDG spec mandates that a relative value in an XDG_* variable is ignored.
        var configured = Environment.GetEnvironmentVariable(variable);
        if (!string.IsNullOrWhiteSpace(configured) && Path.IsPathRooted(configured))
        {
            return configured;
        }

        return Path.Combine(GetHomeDirectory(), relativeFallback);
    }
}
