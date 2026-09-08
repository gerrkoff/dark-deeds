using System.Diagnostics;

namespace DD.Tests.Integration.Helpers;

internal static class DockerHelper
{
    private static readonly TimeSpan ProcessTimeout = TimeSpan.FromMinutes(2);

    internal static async Task EnsureImageAsync(string image)
    {
        var dockerInfo = await RunDockerAsync("info");
        if (dockerInfo.ExitCode != 0)
        {
            throw new InvalidOperationException($"Docker is unavailable: {dockerInfo.Output}");
        }

        var imageInspect = await RunDockerAsync("image", "inspect", image);
        if (imageInspect.ExitCode == 0)
            return;

        var imagePull = await RunDockerAsync("pull", image);
        if (imagePull.ExitCode != 0)
        {
            throw new InvalidOperationException($"Unable to pull {image}: {imagePull.Output}");
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

    private sealed record DockerCommandResult(int ExitCode, string Output);
}
