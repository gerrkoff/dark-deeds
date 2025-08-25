using Microsoft.AspNetCore.StaticFiles;

namespace DD.App.Middlewares;

/// <summary>
/// Serves pre-compressed static files (Brotli / Gzip) produced during frontend build.
/// Looks for [requestedPath].br or [requestedPath].gz depending on Accept-Encoding header.
/// Falls back to next middleware if no pre-compressed variant present.
/// </summary>
public class PreCompressedStaticMiddleware(RequestDelegate next, IWebHostEnvironment env)
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
        {
            await next(context);
            return;
        }

        var accept = context.Request.Headers.AcceptEncoding.ToString();
        var prefersBrotli = accept.Contains("br", StringComparison.OrdinalIgnoreCase);
        var prefersGzip = accept.Contains("gzip", StringComparison.OrdinalIgnoreCase);

        if (!prefersBrotli && !prefersGzip)
        {
            await next(context);
            return;
        }

        var path = context.Request.Path.Value;

        if (string.IsNullOrEmpty(path) || path.EndsWith('/'))
        {
            await next(context);
            return;
        }

        // Only target typical static asset extensions (js/css/html/json/svg)
        if (!HasAssetExtension(path))
        {
            await next(context);
            return;
        }

        // Security: disallow path traversal
        if (path.Contains("..", StringComparison.Ordinal))
        {
            await next(context);
            return;
        }

        var webRoot = env.WebRootPath;

        if (string.IsNullOrEmpty(webRoot))
        {
            await next(context);
            return;
        }

        var originalPhysical = Path.Combine(webRoot, path.TrimStart('/'));
        string? chosenFile = null;
        string encoding = string.Empty;

        if (prefersBrotli)
        {
            var br = originalPhysical + ".br";
#pragma warning disable CA3003
            if (File.Exists(br))
#pragma warning restore CA3003
            {
                chosenFile = br;
                encoding = "br";
            }
        }

        if (chosenFile == null && prefersGzip)
        {
            var gz = originalPhysical + ".gz";
#pragma warning disable CA3003
            if (File.Exists(gz))
#pragma warning restore CA3003
            {
                chosenFile = gz;
                encoding = "gzip";
            }
        }

        if (chosenFile == null)
        {
            await next(context);
            return;
        }

        if (!ContentTypeProvider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        context.Response.Headers.ContentEncoding = encoding;
        context.Response.ContentType = contentType;

        // Ensure proxies/CDNs can vary
        context.Response.Headers.Vary = "Accept-Encoding";
        await context.Response.SendFileAsync(chosenFile);
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
