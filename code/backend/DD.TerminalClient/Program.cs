using System.Reflection;
using DD.Shared.TaskText;
using DD.TerminalClient.Details.Api;
using DD.TerminalClient.Details.Logging;
using DD.TerminalClient.Details.Realtime;
using DD.TerminalClient.Details.Storage;
using DD.TerminalClient.Details.Time;
using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Application;
using DD.TerminalClient.Domain.Editing;
using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.SelfTest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Spectre.Console;

namespace DD.TerminalClient;

internal static class Program
{
    private const string ExecutableName = "dd-terminal";

    // Flags that stand alone, and options that require a following value. Any other token, or a value
    // option left without its value, is a usage error - reported instead of being silently ignored so a
    // typo such as "--porfile local" or a value-less "--state-root" can never fall back to the production
    // profile or the real user state directory.
    private static readonly string[] KnownFlags = ["--self-test", "--version", "--help", "-h"];
    private static readonly string[] KnownValueOptions = ["--profile", "--state-root"];

    private static async Task<int> Main(string[] args)
    {
        if (ValidateArgs(args) is { } argumentError)
        {
            Console.Error.WriteLine($"{ExecutableName}: {argumentError}");
            Console.Error.WriteLine($"Run '{ExecutableName} --help' for usage.");
            return 1;
        }

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

        if (HasOption(args, "--self-test"))
        {
            var selfTestProfile = GetOptionValue(args, "--profile") ?? "production";
            var selfTestStateRoot = GetOptionValue(args, "--state-root");
            return await RunSelfTestAsync(selfTestProfile, selfTestStateRoot);
        }

        if (Console.IsInputRedirected || Console.IsOutputRedirected)
        {
            Console.Error.WriteLine(
                $"{ExecutableName} requires an interactive terminal. " +
                "Run it directly in a TTY, or pass --help, --version, or --self-test for noninteractive output.");
            return 1;
        }

        var profileName = GetOptionValue(args, "--profile") ?? "production";
        var stateRoot = GetOptionValue(args, "--state-root");
        return await RunInteractiveAsync(profileName, stateRoot);
    }

    private static async Task<int> RunInteractiveAsync(string profileName, string? stateRoot)
    {
        var paths = new ApplicationPathProvider(stateRoot);
        if (!new ProfileStore(paths).TryResolve(profileName, out var profile))
        {
            Console.Error.WriteLine($"{ExecutableName}: unknown profile '{profileName}'.");
            return 1;
        }

        var clientId = Guid.NewGuid().ToString();
        var gate = new object();
        string? currentToken = null;

        string? GetToken()
        {
            lock (gate)
            {
                return currentToken;
            }
        }

        void SetToken(string? value)
        {
            lock (gate)
            {
                currentToken = value;
            }
        }

#pragma warning disable CA2000 // HttpClient owns and disposes the handler chain when it is disposed.
        using var httpClient = new HttpClient(
            new TerminalHttpHandler(GetToken, clientId) { InnerHandler = new SocketsHttpHandler() })
        {
            BaseAddress = profile.BaseUri,
            Timeout = TimeSpan.FromSeconds(30),
        };
#pragma warning restore CA2000

        var dates = new SystemLocalDateProvider(TimeProvider.System);
        var parser = new TaskTextParser(dates);
        var formatter = new TaskTextFormatter(dates);

        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new TerminalFileLoggerProvider(paths, profile.Name, TimeProvider.System)));

        var console = AnsiConsole.Console;
        var dependencies = new TerminalDependencies
        {
            Auth = new AuthApiClient(httpClient),
            Tasks = new TaskApiClient(httpClient, dates),
            HubFactory = sink => new TaskHubClient(
                new SignalRTaskHubConnectionFactory(profile.BaseUri.AbsoluteUri, clientId),
                GetToken,
                sink,
                loggerFactory.CreateLogger<TaskHubClient>()),
            StateStore = new LocalStateStore(paths, profile.Name),
            TokenStore = new TokenStore(paths, profile.Name),
            Keys = new TerminalInputReader(console),
            Renderer = new SpectreTerminalRenderer(console),
            Reducer = new ApplicationReducer(
                new TerminalInputReducer(parser, formatter),
                new OverviewProjectionService(dates),
                parser,
                formatter,
                () => Guid.NewGuid().ToString()),
            LocalDate = dates,
            Clock = TimeProvider.System,
            SetToken = SetToken,
            ReadDimensions = () => ViewportRenderable.ResolveDimensions(console, ViewportRenderable.ReadWindowSize),
            ProfileName = profile.Name,
        };

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        var application = new TerminalApplication(dependencies);
        await application.RunAsync(cts.Token);

        if (application.FatalMessage is { } message)
        {
            Console.Error.WriteLine(message);
            return 1;
        }

        return 0;
    }

    private static async Task<int> RunSelfTestAsync(string profileName, string? stateRoot)
    {
        if (!TerminalSelfTest.TryReadCredentials(
            Environment.GetEnvironmentVariable, out var credentials, out var credentialsError))
        {
            Console.Error.WriteLine($"{ExecutableName}: {credentialsError}");
            return TerminalSelfTest.UsageExitCode;
        }

        // Isolate all state in a throwaway directory unless the caller pins one, so a self-test run never
        // touches the user's real profiles, cache, or logs.
        var ownsTemporaryRoot = string.IsNullOrWhiteSpace(stateRoot);
        var resolvedRoot = ownsTemporaryRoot
            ? Path.Combine(Path.GetTempPath(), $"dd-terminal-self-test-{Guid.NewGuid():N}")
            : stateRoot!;

        var paths = new ApplicationPathProvider(resolvedRoot);
        if (!new ProfileStore(paths).TryResolve(profileName, out var profile))
        {
            Console.Error.WriteLine($"{ExecutableName}: unknown profile '{profileName}'.");
            return TerminalSelfTest.UsageExitCode;
        }

        // Distinct client ids so the server does not exclude the observer from the writer's own save
        // notifications; both connections and the REST client share the one token set after sign-in.
        var writerClientId = Guid.NewGuid().ToString();
        var observerClientId = Guid.NewGuid().ToString();
        var gate = new object();
        string? currentToken = null;

        string? GetToken()
        {
            lock (gate)
            {
                return currentToken;
            }
        }

        void SetToken(string? value)
        {
            lock (gate)
            {
                currentToken = value;
            }
        }

#pragma warning disable CA2000 // HttpClient owns and disposes the handler chain when it is disposed.
        using var httpClient = new HttpClient(
            new TerminalHttpHandler(GetToken, writerClientId) { InnerHandler = new SocketsHttpHandler() })
        {
            BaseAddress = profile.BaseUri,
            Timeout = TimeSpan.FromSeconds(30),
        };
#pragma warning restore CA2000

        var dates = new SystemLocalDateProvider(TimeProvider.System);
        var baseUrl = profile.BaseUri.AbsoluteUri;

        var context = new SelfTestContext
        {
            Auth = new AuthApiClient(httpClient),
            Writer = new TaskApiClient(httpClient, dates),
            WriterHubFactory = sink => new TaskHubClient(
                new SignalRTaskHubConnectionFactory(baseUrl, writerClientId),
                GetToken,
                sink,
                NullLogger<TaskHubClient>.Instance),
            ObserverHubFactory = sink => new TaskHubClient(
                new SignalRTaskHubConnectionFactory(baseUrl, observerClientId),
                GetToken,
                sink,
                NullLogger<TaskHubClient>.Instance),
            SetToken = SetToken,
            Report = Console.Error.WriteLine,
            Credentials = credentials,
        };

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        try
        {
            return await new TerminalSelfTest(context).RunAsync(cts.Token);
        }
        finally
        {
            if (ownsTemporaryRoot)
            {
                TryDeleteDirectory(resolvedRoot);
            }
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // Best effort: a leftover temporary directory is harmless.
        }
    }

    private static string? GetOptionValue(string[] args, string option)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], option, StringComparison.Ordinal))
            {
                return args[i + 1];
            }
        }

        return null;
    }

    private static string? ValidateArgs(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (Array.Exists(KnownFlags, flag => string.Equals(arg, flag, StringComparison.Ordinal)))
            {
                continue;
            }

            if (Array.Exists(KnownValueOptions, option => string.Equals(arg, option, StringComparison.Ordinal)))
            {
                if (i + 1 >= args.Length)
                {
                    return $"option '{arg}' requires a value.";
                }

                i++;
                continue;
            }

            return $"unknown option '{arg}'.";
        }

        return null;
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
