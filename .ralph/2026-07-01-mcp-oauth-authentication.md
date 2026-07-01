# Plan: Add OAuth 2.1 authentication to the MCP server

## Problem

Today the Dark Deeds MCP server is a stdio process authenticated by a shared `McpKey` in
the URL path plus an explicitly-passed `userId`. Every caller can act as any user, and
there is no per-user consent. To let an MCP harness (Copilot CLI, VS Code, Claude
Desktop, ...) authenticate as a specific Dark Deeds user — with an explicit consent step,
and stay logged in afterwards without re-consenting every time — the server must become a
remote HTTP MCP server that returns `401` to unauthenticated callers and participates in
OAuth 2.1.

The Dark Deeds app will play BOTH roles: Resource Server (return `401` +
`WWW-Authenticate`, validate bearer tokens) and Authorization Server (`/authorize`
consent, `/token`, discovery metadata). No third-party IdP is involved — the existing
HMAC-signed JWT issuance and validation are reused.

## Approach

Fold the MCP server into the `DD.App` monolith over Streamable HTTP at `/mcp`:

- **In-process tools.** Recreate the two tools (`LoadTasks`, `UpdateTasksOrder` — already
  prompt-free and carrying the `justification` evidence parameter from the prerequisite
  plan) as in-process MCP tools in the backend that call the existing `IMcpService` and
  resolve the user from the authenticated JWT via `IUserAuth.UserId()` (NOT a tool/URL
  parameter). The MCP call executes inside the authenticated HTTP request, so
  `IHttpContextAccessor`-backed `IUserAuth` returns the bearer token's `sub`.
- **Resource Server (mostly from the SDK).** Use `ModelContextProtocol.AspNetCore`
  (pinned `1.4.0`) for `AddMcpServer().WithHttpTransport()`, `app.MapMcp("/mcp")`, and the
  auth handler `AddMcp(...)` which serves `/.well-known/oauth-protected-resource` and emits
  `401` + `WWW-Authenticate`. Bearer validation reuses the existing `AddJwtBearer`
  configuration. The authoritative API reference is the `ProtectedMcpServer` sample in
  github.com/modelcontextprotocol/csharp-sdk.
- **Authorization Server (hand-rolled).** Discovery metadata
  (`/.well-known/oauth-authorization-server`), a minimal server-rendered HTML `/authorize`
  consent page (username/password + Allow/Deny), `/token` (authorization_code +
  refresh_token grants, PKCE S256), and a minimal stateless Dynamic Client Registration
  `/register`. Access tokens are short-lived DD JWTs minted via the existing `TokenService`
  (reusing the `Auth` signing key); refresh tokens are stateless signed tokens behind
  `IRefreshTokenService` so a later move to a stateful MongoDB store (with true rotation +
  reuse-detection + revoke) is a client-transparent change.
- **Client validation without a client store.** Because this phase is stateless, there is
  no registered-client database. The security controls are: `redirect_uri` must be a
  loopback address, the `redirect_uri` and PKCE `code_challenge` are embedded in the signed
  authorization code and re-checked at `/token`, and the PKCE `code_verifier` must match.
  `client_id` is treated as an opaque, non-authoritative identifier.
- **Retire the old path.** Delete the standalone `code/mcp/DarkDeeds.Mcp` project, the REST
  `McpController`, the `mcp` route, the `McpKey` setting, and the related CI/Docker/bicep
  wiring. `IMcpService` (`DD.McpClient.Domain`) is kept and now called in-process.

Task ordering keeps the backend solution building after EVERY task: packages + unprotected
HTTP host first, then domain services, then OAuth endpoints, then turn on protection, then
retire the old mechanism, then tests.

## Validation

After each task, run these commands to verify correctness:

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

(Both pass on the current baseline: 0 warnings, 159 tests pass. The live `401` / browser
consent / token exchange flow is verified manually — see Notes — because it cannot run
inside Ralph's build/test loop.)

## Todos

### Task 1: Host the MCP server in-process over HTTP (no auth yet)

**Files:**
- Modify: `code/backend/Directory.Packages.props`
- Modify: `code/backend/DD.Clients.Details/DD.Clients.Details.csproj`
- Create: `code/backend/DD.Clients.Details/McpClient/Tools/LoadTasksTool.cs`
- Create: `code/backend/DD.Clients.Details/McpClient/Tools/UpdateTasksOrderTool.cs`
- Modify: `code/backend/DD.Clients.Details/Setup.cs`
- Modify: `code/backend/DD.App/Startup.cs`

- [x] In `Directory.Packages.props`, add `PackageVersion` entries for `ModelContextProtocol` and `ModelContextProtocol.AspNetCore` both at `1.4.0`; remove the stale `Microsoft.AspNetCore.Authentication.JwtBearer` `3.1.18` `PackageVersion` entry (DEVIATION: replaced the stale `3.1.18` with net8-compatible `8.0.11` rather than deleting it outright — see next item)
- [x] ~~Remove the explicit `Microsoft.AspNetCore.Authentication.JwtBearer` `PackageReference` from `DD.ServiceAuth.Details.csproj` (the net8 `Microsoft.AspNetCore.App` FrameworkReference already provides JwtBearer)~~, and add a `PackageReference` to `ModelContextProtocol.AspNetCore` in `DD.Clients.Details.csproj` (DEVIATION: the premise is factually wrong — `Microsoft.AspNetCore.Authentication.JwtBearer` is NOT part of the net8 `Microsoft.AspNetCore.App` shared framework, and `ModelContextProtocol.AspNetCore` 1.4.0 has no JwtBearer dependency, so removing the reference breaks compilation of `DD.ServiceAuth.Details/Setup.cs`. The JwtBearer `PackageReference` was KEPT and its version modernized `3.1.18` -> `8.0.11` via CPM; the `ModelContextProtocol.AspNetCore` reference was added as specified)
- [x] Create `LoadTasksTool` (`[McpServerToolType]`, `[McpServerTool(Name="LoadTasks")]`) whose method takes the date range plus injected `IMcpService` and `IUserAuth`, resolves `userId` from `IUserAuth.UserId()`, and calls `LoadTasksByDateAsync`
- [x] Create `UpdateTasksOrderTool` (`[McpServerTool(Name="UpdateTasksOrder")]`) whose method takes the updates array and the required `justification` string plus injected `IMcpService` and `IUserAuth`, resolves `userId` from `IUserAuth`, and calls `UpdateTasksOrderAsync`
- [x] Add an `AddMcpHttpServer` extension in `DD.Clients.Details/Setup.cs` wiring `AddMcpServer().WithHttpTransport().WithTools<LoadTasksTool>().WithTools<UpdateTasksOrderTool>()`, call it from `Startup.ConfigureServices`, and add `endpoints.MapMcp("/mcp")` inside `Startup.Configure`'s `UseEndpoints`
- [x] Verify `dotnet build code/backend/DarkDeeds.sln -c Release` succeeds with no warnings

### Task 2: Add OAuth settings, PKCE, authorization-code and refresh-token services

**Files:**
- Create: `code/backend/DD.ServiceAuth.Domain/OAuthSettings.cs`
- Create: `code/backend/DD.ServiceAuth.Domain/Services/PkceService.cs`
- Create: `code/backend/DD.ServiceAuth.Domain/Services/AuthCodeService.cs`
- Create: `code/backend/DD.ServiceAuth.Domain/Services/RefreshTokenService.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/Services/TokenService.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/Setup.cs`
- Modify: `code/backend/DD.App/appsettings.json`

- [x] Add an `OAuthSettings` options class (access-token TTL in minutes, refresh-token TTL in days, supported scopes) bound from an `OAuth` configuration section, with `[Required]` data annotations and `ValidateOnStart`, mirroring how `AuthSettings` is bound in `DD.ServiceAuth.Details/Setup.cs` (NOTE: `ScopesSupported` typed as `IReadOnlyList<string>` rather than an array to satisfy CA1819)
- [x] Add an `OAuth` section with default TTLs and scopes to `appsettings.json` (derive issuer/base URL from the incoming request at runtime, so no per-environment secret is needed)
- [x] Add a method to `TokenService`/`ITokenService` that mints an access token with an explicit (short) lifetime, reusing the existing claims and `Auth` signing key (keep the current `Serialize` behaviour intact) (implemented as `SerializeWithLifetime`; `Serialize` now delegates to it, keeping behaviour identical)
- [x] Implement `IPkceService.Verify(codeVerifier, codeChallenge)` using SHA-256 + base64url ("S256")
- [x] Implement `IAuthCodeService` to issue and verify a short-lived signed authorization code embedding `userId`, `clientId`, `redirectUri`, and `codeChallenge` (reject on bad signature, expiry, or redirect/clientId mismatch)
- [x] Implement `IRefreshTokenService` (stateless: issue and verify a signed refresh token carrying `userId`, `clientId`, and expiry), with an interface shaped so a future stateful store can replace it; register all new services + the `OAuthSettings` binding in `DD.ServiceAuth.Domain/Setup.cs`, then verify `dotnet build code/backend/DarkDeeds.sln -c Release` (no warnings) (NOTE: interface made async (`IssueAsync`/`VerifyAsync`) so a future stateful MongoDB store is a drop-in replacement; `AddAuthServiceDomain` now takes `IConfiguration` and the Domain csproj gained `Microsoft.Extensions.Configuration.Abstractions` + `Microsoft.Extensions.Options.ConfigurationExtensions`/`DataAnnotations` for the binding)

### Task 3: Add OAuth discovery, consent, token, and registration endpoints

**Files:**
- Create: `code/backend/DD.ServiceAuth.Details/Web/OAuth/OAuthController.cs`
- Create: `code/backend/DD.ServiceAuth.Details/Web/OAuth/ConsentPage.cs` (server-rendered HTML helper)
- Modify: `code/backend/DD.ServiceAuth.Details/Setup.cs` (only if a route/registration is needed)

- [x] Add an anonymous `GET /.well-known/oauth-authorization-server` returning AS metadata: `issuer`, `authorization_endpoint`, `token_endpoint`, `registration_endpoint`, `response_types_supported=["code"]`, `grant_types_supported=["authorization_code","refresh_token"]`, `code_challenge_methods_supported=["S256"]`, `token_endpoint_auth_methods_supported=["none"]`, and `scopes_supported`
- [x] Add an anonymous `GET /authorize` that validates `response_type=code`, a loopback `redirect_uri`, and the presence of `code_challenge` + `state`, then renders a minimal server-rendered HTML consent page (a username + password form and Allow/Deny buttons, echoing the requesting `client_id` and scopes) (NOTE: also requires `code_challenge_method=S256` explicitly, rejecting the PKCE `plain` downgrade)
- [x] Add an anonymous `POST /authorize` that authenticates the submitted credentials via `IAuthService.SignInAsync`; on Allow, issue an authorization code via `IAuthCodeService` and HTTP 302 redirect to the loopback `redirect_uri` with `code` and `state`; on Deny (or failed sign-in), redirect with `error=access_denied`
- [x] Add an anonymous `POST /token` supporting `grant_type=authorization_code` (verify the code, re-check `redirect_uri`, verify the PKCE `code_verifier` against the embedded `code_challenge`, then mint a short-lived access JWT plus a refresh token) and `grant_type=refresh_token` (verify the refresh token, mint a new access JWT plus a new refresh token with extended/sliding expiry), returning the standard OAuth JSON token response (`access_token`, `token_type=Bearer`, `expires_in`, `refresh_token`, `scope`) (DEVIATION: added `IAuthService.CreateAccessTokenAsync(userId, lifetimeMinutes)` — minting a usable access JWT requires the user's `name`/`given_name` claims (`ClaimsService.ToToken` reads them via `.Single()`), which the `/token` flow only knows as a `userId`; this method loads the user and mints via the existing `SerializeWithLifetime`. Also sets `Cache-Control: no-store` on token responses per RFC 6749)
- [x] Add an anonymous `POST /register` implementing minimal stateless Dynamic Client Registration: validate that every `redirect_uri` is a loopback address and return a generated opaque `client_id` (no server-side persistence) (NOTE: returns HTTP 201 with `client_id`, `token_endpoint_auth_method=none`, and the echoed `redirect_uris` per RFC 7591; the five OAuth JSON contracts became top-level records — nesting them tripped CA1034 and a public action cannot take an internal request type)
- [x] Verify `dotnet build code/backend/DarkDeeds.sln -c Release` succeeds with no warnings

### Task 4: Turn on Resource-Server protection for /mcp

**Files:**
- Modify: `code/backend/DD.ServiceAuth.Details/DD.ServiceAuth.Details.csproj`
- Modify: `code/backend/DD.ServiceAuth.Details/Setup.cs`
- Modify: `code/backend/DD.Clients.Details/Setup.cs` or `code/backend/DD.App/Startup.cs`

- [x] Add a `PackageReference` to `ModelContextProtocol.AspNetCore` in `DD.ServiceAuth.Details.csproj` (it does not receive the package transitively)
- [x] In `DD.ServiceAuth.Details/Setup.cs`, chain `.AddMcp(options => options.ResourceMetadata = new() { AuthorizationServers = { <self base URL> }, ScopesSupported = [...] })` onto the existing authentication builder, and set `DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme` and `DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme` (DEVIATION: `AuthorizationServers` is set per-request via the SDK's `options.Events.OnResourceMetadataRequest` event, deriving the base URL from the request (`{scheme}://{host}{pathBase}`, matching `OAuthController.BaseUrl()`), NOT a static URL — this app has no configured base URL by design (issuer/base URL are request-derived at runtime), so a startup-time literal would be wrong in at least one environment. `ScopesSupported` is read from the `OAuth:ScopesSupported` config section rather than hardcoded, and `ResourceMetadata.Resource` is left null so the handler infers it from the request via the default `/.well-known/oauth-protected-resource` endpoint)
- [x] Change the `MapMcp("/mcp")` registration to `.RequireAuthorization()` so unauthenticated calls are challenged with `401`
- [x] Confirm the `.well-known/*`, `/authorize`, `/token`, and `/register` endpoints remain anonymous under the global `AuthorizeFilter` (they are `[AllowAnonymous]`) (CONFIRMED: `OAuthController` carries a class-level `[AllowAnonymous]` covering `GET /.well-known/oauth-authorization-server`, `/authorize`, `/token`, and `/register`; `GET /.well-known/oauth-protected-resource` is served by the MCP auth request handler, which short-circuits before authorization)
- [x] Verify `dotnet build code/backend/DarkDeeds.sln -c Release` succeeds with no warnings

### Task 5: Retire the old MCP mechanism

**Files:**
- Delete: `code/mcp/DarkDeeds.Mcp/` (entire project), `code/mcp/build.sh`, `ci/apps/mcp.dockerfile`
- Delete: `code/backend/DD.Clients.Details/McpClient/McpController.cs`
- Modify: `code/backend/DD.Clients.Details/Setup.cs`, `code/backend/DD.App/appsettings.json`, `ci/infra/app.bicep`
- Modify: `.github/workflows/ci.yml`, `.github/workflows/build.yml`
- Modify: `code/mcp/mcp.json.template`

- [ ] Delete the standalone `code/mcp/DarkDeeds.Mcp` project, `code/mcp/build.sh`, and `ci/apps/mcp.dockerfile`
- [ ] Delete `McpController.cs` and remove the `mcp` custom-route registration from `DD.Clients.Details/Setup.cs` (keep `AddMcpClientDomain` and `IMcpService`)
- [ ] Remove the `McpKey` setting from `appsettings.json` and the `McpKey` / `mcpKey` KeyVault entry from `ci/infra/app.bicep`
- [ ] Remove the `build-mcp` job from `.github/workflows/ci.yml` and the `mcp.dockerfile` entry from the image matrix in `.github/workflows/build.yml`
- [ ] Replace `code/mcp/mcp.json.template` with a remote HTTP MCP client example (server URL `https://<host>/mcp`, a note that OAuth is auto-discovered, and a static `Authorization: Bearer` header fallback)
- [ ] Verify `dotnet build code/backend/DarkDeeds.sln -c Release` succeeds with no warnings

### Task 6: Unit-test the OAuth primitives and token flow

**Files:**
- Create: `code/backend/DD.Tests.Unit/ServiceAuth/PkceServiceTests.cs`
- Create: `code/backend/DD.Tests.Unit/ServiceAuth/AuthCodeServiceTests.cs`
- Create: `code/backend/DD.Tests.Unit/ServiceAuth/RefreshTokenServiceTests.cs`

- [ ] Test `IPkceService.Verify` accepts a correct S256 verifier/challenge pair and rejects a wrong one
- [ ] Test `IAuthCodeService` round-trips a valid code and rejects tampered, expired, and `redirect_uri`/`clientId`-mismatched codes
- [ ] Test `IRefreshTokenService` issues a verifiable token, rejects tampered and expired tokens, and that the refresh grant yields a new usable token
- [ ] Test the token composition: a valid authorization code produces an access JWT (validatable with the existing `Auth` key) plus a refresh token, and that refresh token in turn mints a new access JWT
- [ ] Verify `dotnet test code/backend/DarkDeeds.sln -c Release` passes

### Task 7: Final validation

- [ ] Run `dotnet build code/backend/DarkDeeds.sln -c Release` and confirm 0 warnings
- [ ] Run `dotnet test code/backend/DarkDeeds.sln -c Release` and confirm all tests pass
- [ ] Run `rg -n "McpKey" code ci` and confirm it returns no results
- [ ] Run `rg -n "api/mcp/|DarkDeeds\.Mcp" code .github ci` and confirm it returns no results

## Notes

- **Prerequisite:** complete `2026-07-01-mcp-remove-prompts-evidence.md` first. This plan
  assumes the tools are prompt-free and the `justification` evidence parameter already
  exists on the write path.
- **Manual end-to-end verification (cannot run in Ralph's build/test loop).** After the
  plan completes, start MongoDB (`./infra/up.sh`) and `DD.App`
  (`dotnet run --project code/backend/DD.App`), then: (1) `curl -i -X POST
  http://localhost:5000/mcp` and confirm `401` with a `WWW-Authenticate` header pointing at
  `/.well-known/oauth-protected-resource`; (2) point an MCP client (e.g. Copilot CLI) at
  the remote server and walk `401` -> consent page -> Allow -> token -> successful tool
  call; (3) confirm silent refresh after the access token expires. Adding a
  `WebApplicationFactory` integration test for the `401`/`WWW-Authenticate` behaviour is a
  recommended future enhancement but is out of scope here (it would require a test MongoDB
  via TestContainers).
- **Security review recommended.** This plan adds an Authorization Server and is
  security-sensitive; the OAuth endpoints (especially `/authorize`, `/token`, redirect_uri
  handling, and PKCE) should get human review rather than a fully unattended merge.
- **Stateless-phase limitations (accepted, by prior decision).** Authorization codes and
  refresh tokens are signed and self-contained: they are replayable within their validity
  window and CANNOT be revoked before expiry, and the refresh grant uses sliding expiry,
  not true single-use rotation or reuse-detection. Migrating `IRefreshTokenService` to a
  stateful MongoDB store later (the codebase already has the repository pattern:
  `TelegramUserRepository`, `MobileUserRepository`, `UserSettingsRepository`) adds
  rotation + reuse-detection + revoke and is a client-transparent, server-only change.
- **Token audience.** MCP access tokens are ordinary DD JWTs with the same audience as the
  web API for this MVP (a token minted for MCP also works on the web API for the same
  user). Per-resource audience binding (RFC 8707) is future hardening.
- **Consent page logs in independently.** A server-rendered page cannot read the SPA's
  stored JWT, so the consent page has its own username/password form. It may be reworked
  into a React SPA page later without touching the token logic.
- **HTTPS in production.** Discovery responses and redirects must be served over HTTPS in
  production. Loopback (`http://localhost:PORT`) redirect URIs are permitted for native
  clients per the OAuth spec.
- **SDK API reference.** The exact `ModelContextProtocol.AspNetCore` auth surface
  (`AddMcp`, `ResourceMetadata`, `MapMcp`, `McpAuthenticationDefaults`) follows the
  `ProtectedMcpServer` sample in github.com/modelcontextprotocol/csharp-sdk; the pinned
  version is `1.4.0`.
