using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace DD.Tests.Integration.Infrastructure;

public sealed class IntegrationEnvironment : IAsyncDisposable
{
    private const string MongoImage = "mongo:4.4";
    private const string ContainerNamePrefix = "dd-integration-tests-";
    private static readonly TimeSpan ProcessTimeout = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan CleanupTimeout = TimeSpan.FromSeconds(30);
    private static readonly Lazy<Task<IntegrationEnvironment>> Shared =
        new(CreateAsync, LazyThreadSafetyMode.ExecutionAndPublication);

    private static int _callbacksRegistered;
    private static int _cleanupStarted;

    private readonly MongoDbContainer _mongoContainer;
    private readonly DarkDeedsWebApplicationFactory _factory;
    private readonly string _databaseConnectionString;
    private int _resourcesDisposed;

    private IntegrationEnvironment(
        MongoDbContainer mongoContainer,
        DarkDeedsWebApplicationFactory factory,
        string containerName,
        string databaseName,
        string databaseConnectionString)
    {
        _mongoContainer = mongoContainer;
        _factory = factory;
        _databaseConnectionString = databaseConnectionString;
        ContainerName = containerName;
        DatabaseName = databaseName;
    }

    public string ContainerName { get; }

    public string DatabaseName { get; }

    public static Task<IntegrationEnvironment> GetAsync()
    {
        RegisterCleanupCallbacks();
        return Shared.Value;
    }

    public HttpClient CreateClient()
    {
        return _factory.CreateTestClient();
    }

    public static string CreateUniqueUsername(string prefix = "test")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    public static string CreateUniqueTaskUid()
    {
        return Guid.NewGuid().ToString();
    }

    public static string CreateUniqueIdentifier(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeResourcesAsync();
    }

    private static async Task<IntegrationEnvironment> CreateAsync()
    {
        await EnsureDockerAndMongoImageAsync();

        MongoDbContainer? mongoContainer = null;
        DarkDeedsWebApplicationFactory? factory = null;
        string? databaseName = null;
        string? databaseConnectionString = null;

        try
        {
            var containerName = $"{ContainerNamePrefix}{Guid.NewGuid():N}";
            mongoContainer = new MongoDbBuilder(MongoImage)
                .WithName(containerName)
                .Build();

            await mongoContainer.StartAsync();

            databaseName = $"dd_integration_{Guid.NewGuid():N}";
            databaseConnectionString = CreateDatabaseConnectionString(
                mongoContainer.GetConnectionString(),
                databaseName);
            factory = new DarkDeedsWebApplicationFactory(databaseConnectionString);

            using var client = factory.CreateTestClient();

            return new IntegrationEnvironment(
                mongoContainer,
                factory,
                containerName,
                databaseName,
                databaseConnectionString);
        }
        catch (Exception exception)
        {
            await DisposePartialResourcesAsync(
                mongoContainer,
                factory,
                databaseName,
                databaseConnectionString);
            ExceptionDispatchInfo.Capture(exception).Throw();
            throw;
        }
    }

    private static string CreateDatabaseConnectionString(string connectionString, string databaseName)
    {
        return new MongoUrlBuilder(connectionString)
        {
            AuthenticationSource = "admin",
            DatabaseName = databaseName,
        }.ToString();
    }

    private static void RegisterCleanupCallbacks()
    {
        if (Interlocked.Exchange(ref _callbacksRegistered, 1) != 0)
            return;

        AssemblyLoadContext.Default.Unloading += _ => CleanupAtProcessExit();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => CleanupAtProcessExit();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Process teardown must report cleanup failures without escaping the process-exit callback.")]
    private static void CleanupAtProcessExit()
    {
        if (Interlocked.Exchange(ref _cleanupStarted, 1) != 0 || !Shared.IsValueCreated)
            return;

        try
        {
            var environment = Shared.Value.WaitAsync(CleanupTimeout).GetAwaiter().GetResult();
            environment.DisposeResourcesAsync().WaitAsync(CleanupTimeout).GetAwaiter().GetResult();
        }
        catch (Exception exception)
        {
            ReportCleanupFailure(exception);
        }
    }

    private async Task DisposeResourcesAsync()
    {
        if (Interlocked.Exchange(ref _resourcesDisposed, 1) != 0)
            return;

        await DisposePartialResourcesAsync(
            _mongoContainer,
            _factory,
            DatabaseName,
            _databaseConnectionString);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Resource cleanup must attempt every teardown step and report failures without hiding the original startup failure.")]
    private static async Task DisposePartialResourcesAsync(
        MongoDbContainer? mongoContainer,
        DarkDeedsWebApplicationFactory? factory,
        string? databaseName,
        string? databaseConnectionString)
    {
        try
        {
            if (factory is not null)
            {
                await factory.DisposeAsync().AsTask().WaitAsync(CleanupTimeout);
            }
        }
        catch (Exception exception)
        {
            ReportCleanupFailure(exception);
        }

        try
        {
            if (!string.IsNullOrEmpty(databaseName) && !string.IsNullOrEmpty(databaseConnectionString))
            {
                using var mongoClient = new MongoClient(databaseConnectionString);
                await mongoClient.DropDatabaseAsync(databaseName).WaitAsync(CleanupTimeout);
            }
        }
        catch (Exception exception)
        {
            ReportCleanupFailure(exception);
        }
        finally
        {
            try
            {
                if (mongoContainer is not null)
                {
                    await mongoContainer.DisposeAsync().AsTask().WaitAsync(CleanupTimeout);
                }
            }
            catch (Exception exception)
            {
                ReportCleanupFailure(exception);
            }
        }
    }

    private static async Task EnsureDockerAndMongoImageAsync()
    {
        var dockerInfo = await RunDockerAsync("info");
        if (dockerInfo.ExitCode != 0)
        {
            throw new InvalidOperationException($"Docker is unavailable: {dockerInfo.Output}");
        }

        var imageInspect = await RunDockerAsync("image", "inspect", MongoImage);
        if (imageInspect.ExitCode == 0)
            return;

        var imagePull = await RunDockerAsync("pull", MongoImage);
        if (imagePull.ExitCode != 0)
        {
            throw new InvalidOperationException($"Unable to pull {MongoImage}: {imagePull.Output}");
        }
    }

    private static async Task<DockerCommandResult> RunDockerAsync(params string[] arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        foreach (var argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        if (!process.Start())
            throw new InvalidOperationException("Could not start the docker process.");

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        try
        {
            await process.WaitForExitAsync().WaitAsync(ProcessTimeout);
        }
        catch (TimeoutException)
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);

            await process.WaitForExitAsync();
            throw new TimeoutException($"The docker command timed out: docker {string.Join(' ', arguments)}");
        }

        var output = await outputTask;
        var error = await errorTask;
        return new DockerCommandResult(process.ExitCode, $"{output}{error}".Trim());
    }

    private static void ReportCleanupFailure(Exception exception)
    {
        Console.Error.WriteLine($"Integration test cleanup failed: {exception}");
    }

    private sealed record DockerCommandResult(int ExitCode, string Output);
}
