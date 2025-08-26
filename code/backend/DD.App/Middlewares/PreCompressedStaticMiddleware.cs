using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;

namespace DD.App.Middlewares;

/// <summary>
/// Serves pre-compressed static files (Brotli / Gzip) produced during frontend build.
/// Looks for [requestedPath].br or [requestedPath].gz depending on Accept-Encoding header.
/// Falls back to next middleware if no pre-compressed variant present.
/// </summary>
[SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "test")]
public class PreCompressedStaticMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<PreCompressedStaticMiddleware> logger)
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path.Value;
        var acceptEncodingHeader = context.Request.Headers.AcceptEncoding.ToString();
        logger.LogDebug("[PreCompressedStatic] Start requestId={RequestId} method={Method} path={Path} acceptEncoding={AcceptEncoding}", requestId, method, path, acceptEncodingHeader);

        if (!HttpMethods.IsGet(method) && !HttpMethods.IsHead(method))
        {
            logger.LogDebug("[PreCompressedStatic] Skip: method not GET/HEAD requestId={RequestId} method={Method}", requestId, method);
            await next(context);
            return;
        }

        var prefersBrotli = acceptEncodingHeader.Contains("br", StringComparison.OrdinalIgnoreCase);
        var prefersGzip = acceptEncodingHeader.Contains("gzip", StringComparison.OrdinalIgnoreCase);
        logger.LogDebug("[PreCompressedStatic] Enc prefs requestId={RequestId} prefersBrotli={PrefersBrotli} prefersGzip={PrefersGzip}", requestId, prefersBrotli, prefersGzip);

        if (!prefersBrotli && !prefersGzip)
        {
            logger.LogDebug("[PreCompressedStatic] Skip: no br/gzip in Accept-Encoding requestId={RequestId}", requestId);
            await next(context);
            return;
        }

        if (string.IsNullOrEmpty(path) || path.EndsWith('/'))
        {
            logger.LogDebug("[PreCompressedStatic] Skip: empty or ends with slash requestId={RequestId} path={Path}", requestId, path);
            await next(context);
            return;
        }

        // Only target typical static asset extensions (js/css/html/json/svg)
        if (!HasAssetExtension(path))
        {
            logger.LogDebug("[PreCompressedStatic] Skip: unsupported extension requestId={RequestId} path={Path}", requestId, path);
            await next(context);
            return;
        }

        // Security: disallow path traversal
        if (path.Contains("..", StringComparison.Ordinal))
        {
            logger.LogWarning("[PreCompressedStatic] Abort: path traversal attempt requestId={RequestId} path={Path}", requestId, path);
            await next(context);
            return;
        }

        var webRoot = env.WebRootPath;
        logger.LogTrace("[PreCompressedStatic] WebRootPath resolved requestId={RequestId} webRoot={WebRoot}", requestId, webRoot);

        if (string.IsNullOrEmpty(webRoot))
        {
            logger.LogWarning("[PreCompressedStatic] Skip: webRoot missing requestId={RequestId}", requestId);
            await next(context);
            return;
        }

        var originalPhysical = Path.Combine(webRoot, path.TrimStart('/'));
        string? chosenFile = null;
        var encoding = string.Empty;
        logger.LogTrace("[PreCompressedStatic] Candidate base file requestId={RequestId} physical={Physical}", requestId, originalPhysical);

        if (prefersBrotli)
        {
            var br = originalPhysical + ".br";
#pragma warning disable CA3003
            var brExists = File.Exists(br);
#pragma warning restore CA3003
            logger.LogDebug("[PreCompressedStatic] Brotli check requestId={RequestId} file={File} exists={Exists}", requestId, br, brExists);
            if (brExists)
            {
                chosenFile = br;
                encoding = "br";
            }
        }

        if (chosenFile == null && prefersGzip)
        {
            var gz = originalPhysical + ".gz";
#pragma warning disable CA3003
            var gzExists = File.Exists(gz);
#pragma warning restore CA3003
            logger.LogDebug("[PreCompressedStatic] Gzip check requestId={RequestId} file={File} exists={Exists}", requestId, gz, gzExists);
            if (gzExists)
            {
                chosenFile = gz;
                encoding = "gzip";
            }
        }

        if (chosenFile == null)
        {
            logger.LogDebug("[PreCompressedStatic] Fallback: no compressed variant found requestId={RequestId} base={Base}", requestId, originalPhysical);
            await next(context);
            return;
        }

        if (!ContentTypeProvider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
            logger.LogTrace("[PreCompressedStatic] Content type defaulted requestId={RequestId} path={Path} contentType={ContentType}", requestId, path, contentType);
        }
        else
        {
            logger.LogTrace("[PreCompressedStatic] Content type resolved requestId={RequestId} path={Path} contentType={ContentType}", requestId, path, contentType);
        }

        context.Response.Headers.ContentEncoding = encoding;
        context.Response.ContentType = contentType;
        context.Response.Headers.Vary = "Accept-Encoding"; // Ensure proxies/CDNs can vary

        logger.LogInformation("[PreCompressedStatic] Serving compressed file requestId={RequestId} path={Path} chosen={Chosen} encoding={Encoding} contentType={ContentType}", requestId, path, chosenFile, encoding, contentType);
        await context.Response.SendFileAsync(chosenFile);
        logger.LogDebug("[PreCompressedStatic] Completed requestId={RequestId}", requestId);
    }

    private static bool HasAssetExtension(string? path)
    {
        if (path == null) return false;

        return path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".mjs", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
    }
}
