using System.Reflection;

namespace DD.TerminalClient;

internal static class Program
{
    private const string ExecutableName = "dd-terminal";

    private static int Main(string[] args)
    {
        if (HasOption(args, "--help") || HasOption(args, "-h"))
        {
            Console.WriteLine(BuildHelpText());
            return 0;
        }

        if (HasOption(args, "--version"))
        {
            Console.WriteLine(GetVersion());
            return 0;
        }

        var selfTest = HasOption(args, "--self-test");

        if (!selfTest && (Console.IsInputRedirected || Console.IsOutputRedirected))
        {
            Console.Error.WriteLine(
                $"{ExecutableName} requires an interactive terminal. " +
                "Run it directly in a TTY, or pass --help, --version, or --self-test for noninteractive output.");
            return 1;
        }

        // The interactive event loop, startup, and self-test are wired up in later iterations.
        Console.WriteLine($"{ExecutableName} is not fully implemented yet.");
        return 0;
    }

    private static bool HasOption(string[] args, string option)
    {
        return Array.Exists(args, arg => string.Equals(arg, option, StringComparison.Ordinal));
    }

    private static string GetVersion()
    {
        var assembly = typeof(Program).Assembly;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrEmpty(informationalVersion))
        {
            var plusIndex = informationalVersion.IndexOf('+', StringComparison.Ordinal);
            return plusIndex >= 0 ? informationalVersion[..plusIndex] : informationalVersion;
        }

        return assembly.GetName().Version?.ToString() ?? "0.0.0";
    }

    private static string BuildHelpText()
    {
        return $"""
            {ExecutableName} - keyboard-first terminal client for Dark Deeds

            Usage:
              {ExecutableName} [options]

            Options:
              --profile <name>   Select a named profile (default: production).
              --self-test        Run the unattended self-test and exit.
              --version          Print version information and exit.
              --help, -h         Print this help text and exit.
            """;
    }
}
