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
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace DD.TerminalClient;

internal static class Program
{
    private const string ExecutableName = "dd-terminal";

    private static async Task<int> Main(string[] args)
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

        if (HasOption(args, "--self-test"))
        {
            Console.Error.WriteLine($"{ExecutableName}: --self-test is not implemented yet.");
            return 1;
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
