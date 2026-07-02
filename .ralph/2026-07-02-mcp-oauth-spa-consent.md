# Plan: Serve the MCP OAuth consent step from the styled SPA

## Problem

The MCP OAuth authorization endpoint (`GET /authorize`) renders a hand-written, unstyled HTML
consent form (`ConsentPageService`) that collects username/password plus Allow/Deny. It looks
nothing like Dark Deeds. We want the consent step served by our own React SPA — same styling and
login form — and to reuse an existing browser session when the user is already signed in, so an
already-authenticated user only has to click once.

Confirmed design decisions with the maintainer:
1. Reuse the browser session: show a one-click **Authorize** screen (with the username) when the
   user is already signed in; show the styled **login form** only when not signed in.
2. Both paths end on ONE consistent explicit **Authorize** screen (with a Deny/Cancel option).
3. The redirect target of `GET /authorize` is a **per-environment configured base URL**
   (dev `http://localhost:3000`, prod same origin), mirroring the existing `OAuth:IssuerBaseUrl`
   setting — required because in non-Production the backend serves Swagger at `/` and the SPA runs
   on a separate origin (`:3000`), so a same-origin `/` redirect would not reach the SPA in dev.

## Approach

Turn `/authorize` into a thin redirect into the SPA and let the SPA drive consent and completion:

- **`GET /authorize`** keeps its OAuth request validation, then 302-redirects to
  `{OAuth:ConsentRedirectBaseUrl}/{query}` (preserving the original query string). The redirect URL
  is built in the Domain layer because `OAuthSettings` is `internal` to `DD.ServiceAuth.Domain`.
  `/token`, `/register`, `/.well-known`, `/mcp` stay backend-served; no `vite.config` denylist
  change is needed.
- **The SPA** detects the OAuth authorize request at bootstrap (`main.tsx`, by parsing the query)
  and renders a dedicated styled consent flow instead of the tab app — isolated from the app's
  task-hub / token-renewal hooks:
  - Signed in (valid JWT in `localStorage`): one-click **Authorize** screen + Deny.
  - Not signed in: the existing styled **login form** (`Signin`); on success the JWT is stored and
    the flow lands on the SAME Authorize screen.
- **Completion** (`POST /authorize`, JSON): the SPA sends its Bearer JWT and
  `{ action, clientId, redirectUri, codeChallenge, state }`, receives `{ redirectUrl }`, and does
  `window.location.assign(redirectUrl)` back to the MCP client. `allow` requires an authenticated
  user (`IUserAuth`) and issues the auth code; `deny` returns `error=access_denied` and never
  touches the user identity; `redirect_uri` is re-validated server-side (`IsAllowedRedirectUri`).
  PKCE and issuer/host handling are unchanged.

In dev the redirect lands on the Vite SPA at `:3000`; its `POST /authorize` back to `:5000` is
already permitted by the dev CORS policy (`SetIsOriginAllowed(origin => origin.EndsWith(":3000"))`,
`AllowAnyHeader`, `AllowCredentials`), so the full flow is verifiable locally.

Task ordering keeps the backend solution building after EVERY task: add the config setting (and fix
the `required`-member test initializers), add the completion DTOs, rework the flow (service +
endpoints together), then delete the dead consent page; then the frontend parser/API, the optional
`Signin` prop, the consent component, and finally the bootstrap branch. Frontend UI is verified by
the mandatory local Selenium e2e regression run plus a curl smoke of the new endpoints — the project
has no React Testing Library (tests are logic-only), matching the repo's existing test convention.

## Validation

Fast per-task gate — Ralph runs this after EVERY task. A clean build has **0 warnings** (warnings
are treated as errors); fix them before moving on:

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

Tasks that touch `code/frontend/**` additionally run the frontend gate (as their final checkbox):

```
cd code/frontend && npm run ci
```

## Todos

### Task 1: Add the OAuth:ConsentRedirectBaseUrl setting and its wiring

**Files:**
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/OAuthSettings.cs`
- Modify: `code/backend/DD.App/appsettings.json`
- Modify: `ci/infra/app.bicep`
- Modify: `ci/infra/app.prod.parameters.json`
- Modify: `code/backend/DD.Tests.Unit/ServiceAuth/TokenFlowTests.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceAuth/RefreshTokenServiceTests.cs`

- [x] Add `[Required] public required string ConsentRedirectBaseUrl { get; init; }` to `OAuthSettings` (mirror the `IssuerBaseUrl` `CA1056` suppression)
- [x] In `appsettings.json` add `"ConsentRedirectBaseUrl": "http://localhost:3000"` under the `OAuth` section (dev default)
- [x] In `ci/infra/app.bicep` add `param oauthConsentRedirectBaseUrl string = ''` and an `OAuth__ConsentRedirectBaseUrl` app-setting entry set to that param (mirror `OAuth__IssuerBaseUrl`)
- [x] In `ci/infra/app.prod.parameters.json` add `"oauthConsentRedirectBaseUrl": { "value": "https://dark-deeds.com" }` (same origin as the issuer in prod)
- [x] Set `ConsentRedirectBaseUrl = "http://localhost:3000"` in both `OAuthSettings` initializers in `TokenFlowTests.cs` and `RefreshTokenServiceTests.cs` (the new member is `required`, so the test project will not compile otherwise)
- [x] Verify the fast checks pass with 0 warnings

### Task 2: Add the OAuth consent completion DTOs

**Files:**
- Create: `code/backend/DD.ServiceAuth.Domain/OAuth/Dto/OAuthAuthorizeRequestDto.cs`
- Create: `code/backend/DD.ServiceAuth.Domain/OAuth/Dto/OAuthRedirectResponseDto.cs`

- [x] Add `public sealed record OAuthAuthorizeRequestDto(string? Action, string? ClientId, string? RedirectUri, string? CodeChallenge, string? State);` (nullable members — bound from the SPA's camelCase JSON, validated manually in the controller; add the `CA1054` suppression used by sibling OAuth DTOs for the `RedirectUri` string)
- [x] Add `public sealed record OAuthRedirectResponseDto(string RedirectUrl);` (add the `CA1054` suppression consistent with siblings)
- [x] Verify the fast checks pass with 0 warnings

### Task 3: Rework the /authorize flow to redirect into the SPA and complete via JSON

**Files:**
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Services/OAuthFlowService.cs`
- Modify: `code/backend/DD.ServiceAuth.Details/Web/Controllers/OAuthController.cs`

- [x] In `IOAuthFlowService`/`OAuthFlowService` add `string BuildConsentRedirect(string queryString)` returning `$"{_oauthSettings.ConsentRedirectBaseUrl}/{queryString}"`
- [x] Replace `AuthorizeAsync(action, username, password, ...)` with `Task<string> BuildAuthorizeRedirectAsync(string action, string userId, string clientId, string redirectUri, string codeChallenge, string state)`: for `deny` return `oauthUrlService.BuildErrorRedirect(redirectUri, OAuthConstants.AccessDeniedError, state)`; for `allow` call `authCodeService.IssueAsync` for `userId` and return `oauthUrlService.BuildSuccessRedirect(...)` — drop the `authService.SignInAsync`/`GetUserIdAsync` calls; remove `RenderConsentPage` and the `IConsentPageService` constructor dependency
- [x] In `OAuthController`, change `GET /authorize` to `return Redirect(oauthFlowService.BuildConsentRedirect(Request.QueryString.Value ?? string.Empty))` after the existing validation (remove the `RenderConsentPage`/`Content(html, ...)` block)
- [x] Inject `IUserAuth` into `OAuthController`; make `POST /authorize` accept `[FromBody] OAuthAuthorizeRequestDto? request`, guard `request is null` and each required field (=> `BadRequest(OAuthErrorDto...)`), and re-validate `redirect_uri` via `IsAllowedRedirectUri` (=> `BadRequest`)
- [x] In `POST /authorize` branch on `action` FIRST: for `deny` build the error redirect without touching the user identity; only for `allow` require `userAuth.IsAuthenticated()` (else `return Unauthorized()`), then read `userAuth.UserId()`, call `BuildAuthorizeRedirectAsync`, and `return Ok(new OAuthRedirectResponseDto(location))` (never call `UserId()`/`AuthToken()` on an unauthenticated request — it throws)
- [x] Verify the fast checks pass with 0 warnings

### Task 4: Delete the server-rendered consent page and its dead error DTOs

**Files:**
- Delete: `code/backend/DD.ServiceAuth.Domain/OAuth/Services/ConsentPageService.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/Setup.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Dto/OAuthErrorDto.cs`

- [ ] Delete `ConsentPageService.cs` (the `IConsentPageService` interface and `ConsentPageService` class)
- [ ] Remove `services.AddTransient<IConsentPageService, ConsentPageService>();` from `Setup.cs`
- [ ] Remove the now-unused `OAuthErrorDto` entries `UsernameRequired`, `PasswordRequired`, and `CodeChallengeAndStateRequired` (confirm each is unreferenced via a repo search before deleting)
- [ ] Verify the fast checks pass with 0 warnings

### Task 5: Add the OAuth request parser, models, and completion API (frontend)

**Files:**
- Create: `code/frontend/src/oauth/models/OAuthAuthorizeRequest.ts`
- Create: `code/frontend/src/oauth/models/OAuthAuthorizeResult.ts`
- Create: `code/frontend/src/oauth/parseOAuthAuthorizeRequest.ts`
- Create: `code/frontend/src/oauth/api/OAuthApi.ts`
- Create: `code/frontend/tests/oauth/parseOAuthAuthorizeRequest.test.ts`

- [ ] Define `interface OAuthAuthorizeRequest { clientId, redirectUri, codeChallenge, state, scope }` and a result union `{ status: 'redirect'; redirectUrl: string } | { status: 'needs-login' } | { status: 'error' }`
- [ ] Implement `parseOAuthAuthorizeRequest(search: string): OAuthAuthorizeRequest | null` returning the typed request only when `response_type=code` and `client_id`, `redirect_uri`, `code_challenge`, `state` are all present, else `null`
- [ ] Implement `OAuthApi.authorize(action: 'allow' | 'deny', request: OAuthAuthorizeRequest)` that `fetch`es `POST ${baseUrlProvider.getBaseUrl()}authorize` with JSON body `{ action, clientId, redirectUri, codeChallenge, state }` and `Authorization: Bearer ${storageService.loadAccessToken()}`; return `{ status: 'redirect', redirectUrl }` on 200, `{ status: 'needs-login' }` on 401, `{ status: 'error' }` otherwise; export an `oauthApi` singleton (mirror the `loginApi` pattern)
- [ ] Add vitest cases for the parser: a valid query returns the parsed request; a missing/blank required param returns `null`; a normal app URL (no OAuth params) returns `null`
- [ ] Verify `cd code/frontend && npm run ci` passes (run `npm run fmt` first if `fmt:check` fails)

### Task 6: Make the Signin signup link optional (frontend)

**Files:**
- Modify: `code/frontend/src/login/components/Signin.tsx`

- [ ] Add an optional `hideSignup?: boolean` prop; when true, render the sign-in form only (omit the "Sign up here" link) and make `switchToSignup` optional / guard its use so it is not required in that mode; leave the existing `Login` caller unchanged (prop defaults to showing signup)
- [ ] Verify `cd code/frontend && npm run ci` passes

### Task 7: Build the OAuthConsent flow component (frontend)

**Files:**
- Create: `code/frontend/src/oauth/OAuthConsent.tsx`
- Create: `code/frontend/src/oauth/components/AuthorizePrompt.tsx`

- [ ] `AuthorizePrompt`: a styled `Card` showing the requesting `clientId` and the signed-in `username`, with **Authorize** and **Deny** buttons and a submitting/disabled state (Bootstrap classes consistent with `Signin`)
- [ ] `OAuthConsent`: take the parsed `OAuthAuthorizeRequest` as a prop; local phase state `login | authorize | submitting`; initialize to `authorize` when `authService.getCurrentUser()` returns a user, else `login`
- [ ] Login phase: render `Signin` with `hideSignup`, wired to local username/password state, the existing `signin` login thunk, and the `login` slice `isLogInPending`/`logInError`; on `SigninResultEnum.Success` call `storageService.saveAccessToken(token)` and switch to `authorize`
- [ ] Authorize phase: render `AuthorizePrompt`; Authorize => `oauthApi.authorize('allow', request)`, Deny => `oauthApi.authorize('deny', request)`; on `redirect` do `window.location.assign(redirectUrl)`; on `needs-login` switch back to `login`; on `error` show an inline alert
- [ ] Verify `cd code/frontend && npm run ci` passes

### Task 8: Branch the app bootstrap into the OAuth consent flow (frontend)

**Files:**
- Modify: `code/frontend/src/main.tsx`

- [ ] Compute `const oauthRequest = parseOAuthAuthorizeRequest(window.location.search)` before rendering
- [ ] Render `<OAuthConsent request={oauthRequest} />` inside the existing `<Provider store={store}>` when `oauthRequest` is non-null, otherwise render `<App />` exactly as today (normal app behavior must be unchanged when there are no OAuth params)
- [ ] Verify `cd code/frontend && npm run ci` passes

### Task 9: Final validation

- [ ] Build the whole backend solution with 0 warnings: `dotnet build code/backend/DarkDeeds.sln -c Release`
- [ ] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`
- [ ] Run the full frontend gate: `cd code/frontend && npm run ci`
- [ ] Bring up the full local stack (see the local-run workflow in `.github/copilot-instructions.md`) as detached background processes (never piped to `head`/`tail`), recording each PID: MongoDB via `./infra/up.sh`; the backend `dotnet run --project code/backend/DD.App` on `:5000` (wait for `curl -fsS http://localhost:5000/healthcheck` => `Healthy`); the frontend `cd code/frontend && npm run dev` on `:3000` (wait for HTTP 200)
- [ ] curl smoke on `:5000`: `GET "/authorize?response_type=code&client_id=x&redirect_uri=http://127.0.0.1:33418&code_challenge=abc&code_challenge_method=S256&state=xyz"` returns 302 with `Location` starting `http://localhost:3000/?` and preserving the query; then `POST /api/auth/account/signin` (test/test) to obtain a JWT, and `POST /authorize` with that Bearer and `{"action":"allow", ...}` returns `{ redirectUrl }` containing `code=` and `state=`, while `{"action":"deny", ...}` returns a `redirectUrl` containing `error=access_denied`
- [ ] Run the e2e suite against the local stack and confirm ALL pass: `dotnet test code/tests/DarkDeeds.E2eTests` (defaults `URL=http://localhost:3000` / `BE_URL=http://localhost:5000`; uses a local ChromeDriver, so Chrome must be installed)
- [ ] Fix any failure and re-run until the backend build/tests, the frontend `npm run ci`, the curl smokes, and the whole e2e suite are ALL green; then tear down the backend and frontend you started (stop each by PID; leave MongoDB running)

## Notes

- The full browser click-through of the OAuth consent (external redirect to a loopback MCP client)
  is verified manually outside Ralph, as in the prior MCP OAuth plan; the curl smokes above cover
  the `GET` redirect and the `allow`/`deny` completion automatically.
- The dev cross-origin completion (`:3000` SPA => `:5000` backend) relies on the existing dev CORS
  policy in `Startup.cs`; no CORS change is needed.
- No `vite.config` `navigateFallbackDenylist` change is needed; `/authorize`, `/token`, `/register`,
  `/.well-known`, `/mcp` stay backend-served and the redirect goes to the configured SPA base URL.
- Everything committed stays in English; do not modify any `// important`-marked code without
  approval.
