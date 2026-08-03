using System.Runtime.Versioning;

namespace DD.TerminalClient.Details.Storage;

// Crash-safe file writes shared by the local-state and token stores. Content is written to a sibling
// temp file, flushed all the way to disk, tightened to Unix 0600 where supported, then atomically
// renamed over the target. Windows files inherit the containing directory's ACL. A partial or failed
// write therefore never corrupts or truncates the previous good file: on any failure the temp file is
// removed and the original is left untouched.
internal static class AtomicFileWriter
{
    internal const UnixFileMode OwnerReadWrite = UnixFileMode.UserRead | UnixFileMode.UserWrite;

    public static void WriteAllBytes(string path, byte[] contents)
    {
        var directory = Path.GetDirectoryName(path)
            ?? throw new ArgumentException($"Path '{path}' has no parent directory.", nameof(path));
        Directory.CreateDirectory(directory);

        var tempPath = path + ".tmp";
        var committed = false;
        try
        {
            WriteTempFile(tempPath, contents);
            TightenToOwnerOnly(tempPath);
            File.Move(tempPath, path, overwrite: true);
            committed = true;
        }
        finally
        {
            if (!committed)
            {
                TryDelete(tempPath);
            }
        }
    }

    public static void TightenToOwnerOnly(string path)
    {
        if (!OperatingSystem.IsWindows())
        {
            SetOwnerOnly(path);
        }
    }

    private static void WriteTempFile(string tempPath, byte[] contents)
    {
        var options = new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.None,
        };

        // Create the temp file owner-only from the start so a token is never briefly world-readable;
        // TightenToOwnerOnly then guarantees the exact mode regardless of the process umask.
        if (!OperatingSystem.IsWindows())
        {
            options.UnixCreateMode = OwnerReadWrite;
        }

        using var stream = new FileStream(tempPath, options);
        stream.Write(contents, 0, contents.Length);
        stream.Flush(flushToDisk: true);
    }

    [UnsupportedOSPlatform("windows")]
    private static void SetOwnerOnly(string path)
    {
        File.SetUnixFileMode(path, OwnerReadWrite);
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
