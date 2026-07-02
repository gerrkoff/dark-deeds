using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace DD.ServiceAuth.Domain.OAuth.Services;

internal interface IConsentPageService
{
    [SuppressMessage(
        "Design",
        "CA1054:URI-like parameters should not be strings",
        Justification = "OAuth redirect_uri is echoed verbatim into a hidden form field and must not be URI-normalized.")]
    string Render(
        string clientId,
        string redirectUri,
        string codeChallenge,
        string state,
        string scope);
}

internal sealed class ConsentPageService : IConsentPageService
{
    [SuppressMessage(
        "Design",
        "CA1054:URI-like parameters should not be strings",
        Justification = "OAuth redirect_uri is echoed verbatim into a hidden form field and must not be URI-normalized.")]
    public string Render(
        string clientId,
        string redirectUri,
        string codeChallenge,
        string state,
        string scope)
    {
        var encClientId = WebUtility.HtmlEncode(clientId);
        var encRedirectUri = WebUtility.HtmlEncode(redirectUri);
        var encCodeChallenge = WebUtility.HtmlEncode(codeChallenge);
        var encState = WebUtility.HtmlEncode(state);
        var encScope = WebUtility.HtmlEncode(scope);

        return $$"""
                 <!DOCTYPE html>
                 <html lang="en">
                 <head>
                     <meta charset="utf-8">
                     <meta name="viewport" content="width=device-width, initial-scale=1">
                     <title>Authorize Dark Deeds access</title>
                     <style>
                         body { font-family: system-ui, sans-serif; max-width: 420px; margin: 40px auto; padding: 0 16px; }
                         h1 { font-size: 1.4rem; }
                         label { display: block; margin: 12px 0 4px; }
                         input[type=text], input[type=password] { width: 100%; padding: 8px; box-sizing: border-box; }
                         .actions { margin-top: 20px; display: flex; gap: 12px; }
                         button { padding: 10px 18px; font-size: 1rem; cursor: pointer; }
                         .allow { background: #2b7a2b; color: #fff; border: none; }
                         .deny { background: #eee; border: 1px solid #ccc; }
                         .scopes { color: #555; }
                     </style>
                 </head>
                 <body>
                     <h1>Authorize access</h1>
                     <p>The application <strong>{{encClientId}}</strong> is requesting access to your Dark Deeds account.</p>
                     <p class="scopes">Requested scopes: {{encScope}}</p>
                     <form method="post">
                         <input type="hidden" name="client_id" value="{{encClientId}}">
                         <input type="hidden" name="redirect_uri" value="{{encRedirectUri}}">
                         <input type="hidden" name="code_challenge" value="{{encCodeChallenge}}">
                         <input type="hidden" name="state" value="{{encState}}">
                         <label for="username">Username</label>
                         <input id="username" type="text" name="username" autocomplete="username" required>
                         <label for="password">Password</label>
                         <input id="password" type="password" name="password" autocomplete="current-password" required>
                         <div class="actions">
                             <button class="allow" type="submit" name="action" value="allow">Allow</button>
                             <button class="deny" type="submit" name="action" value="deny">Deny</button>
                         </div>
                     </form>
                 </body>
                 </html>
                 """;
    }
}
