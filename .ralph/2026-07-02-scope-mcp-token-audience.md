# Plan: Scope MCP OAuth tokens to the /mcp endpoint

## Problem

Dark Deeds issues JWT access tokens two ways: the regular web/login flow
(`AccountController` => `AuthService.SignIn/SignUp/RenewToken`) and the OAuth/MCP flow
(`OAuthController.Token` => `OAuthFlowService.IssueTokensAsync` =>
`AuthService.CreateAccessTokenAsync`). Both mint the token through the same
`TokenService.Serialize`, which hardcodes `Issuer = Auth:Issuer` and
`Audience = Auth:Audience`; the two tokens differ only in lifetime. A single `JwtBearer`
scheme validates every request (`ValidAudience = Auth:Audience`) and `/mcp` forwards
authentication to that same scheme (`AddMcp ForwardAuthenticate = JwtBearer`).

As a result, a token obtained through the MCP OAuth flow is accepted on every
authenticated endpoint (tasks API, `/ws`, account renew), not just MCP tool calls. We
want an MCP token usable **only** for MCP tool calls (`MapMcp("/mcp")`).

## Approach

Resource-scope the OAuth access token by JWT **audience** and validate `/mcp` with a
**dedicated bearer scheme** bound to that audience:

- OAuth-issued access tokens get a distinct audience `dd-oauth-access` instead of the
  shared `Auth:Audience`. All user claims (`sub`/`name`/`given_name`/`jti`/`exp`) are
  unchanged, so `ClaimsService`/`UserAuth` user resolution keeps working. Issuer stays
  `Auth:Issuer` — only the audience discriminates (the standard "who the token is for" =
  the resource).
- A second `JwtBearer` scheme (`McpBearer`) is identical to the default one but validates
  `ValidAudience = dd-oauth-access`. The `/mcp` endpoint authenticates with it via
  `AddMcp ForwardAuthenticate = McpBearer`; its challenge still comes from
  `McpAuthenticationDefaults`, so protected-resource-metadata discovery is unchanged.

Net effect (clean bidirectional separation): an MCP token (aud `dd-oauth-access`) is valid
at `/mcp` and rejected (401) everywhere else; a login token (aud `Auth:Audience`) is valid
on the app endpoints and rejected (401) at `/mcp`. No new configuration is required (the
audience is a compile-time invariant, like the existing internal `OAuthTokenKinds`
audiences), so `appsettings`/bicep/dedicated-host config is untouched.

## Validation

Fast per-task gate (repo rule — run after every task; a clean build has **0 warnings**,
which are treated as errors):

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

## Todos

### Task 1: Issue OAuth access tokens with a distinct MCP audience

**Files:**
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/OAuthConstants.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/Services/TokenService.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/Services/AuthService.cs`

- [x] In `OAuthConstants`, add `public const string AccessTokenAudience = "dd-oauth-access";` with a short comment explaining it is the audience that scopes OAuth access tokens to the MCP resource (public because the JwtBearer setup in `DD.ServiceAuth.Details` must reference it).
- [x] In `ITokenService.Serialize` and `TokenService.Serialize`, add an optional `string? audience = null` parameter and set the descriptor `Audience = audience ?? _authSettings.Audience` (mirrors the existing optional `lifetimeMinutes`).
- [x] In `AuthService.CreateAccessTokenAsync`, pass `OAuthConstants.AccessTokenAudience` as the audience to `tokenService.Serialize` (add `using DD.ServiceAuth.Domain.OAuth;` if needed). The sign-in/sign-up/renew paths keep the default `Auth:Audience`.
- [x] Verify the fast checks pass.

### Task 2: Validate `/mcp` with a dedicated MCP bearer scheme

**Files:**
- Modify: `code/backend/DD.ServiceAuth.Details/Setup.cs`

- [x] Add `using DD.ServiceAuth.Domain.OAuth;` to `Setup.cs` (so `OAuthConstants.AccessTokenAudience` resolves) and a `private const string McpJwtBearerScheme = "McpBearer";` to the `Setup` class.
- [x] Register a second `.AddJwtBearer(McpJwtBearerScheme, options => …)` with the same `RequireHttpsMetadata`, `MapInboundClaims = false`, signing key, `ValidIssuer = authSettings.Issuer`, and `ValidateLifetime` as the default scheme, but `ValidAudience = OAuthConstants.AccessTokenAudience`. Do NOT add the `/ws` `OnMessageReceived` query-token event to this scheme.
- [x] Change the `.AddMcp(...)` `options.ForwardAuthenticate` from `JwtBearerDefaults.AuthenticationScheme` to `McpJwtBearerScheme`.
- [x] Update the stale comment above `ForwardAuthenticate` (it currently says `/mcp` accepts the same access token) to explain `/mcp` now only accepts the `dd-oauth-access` audience.
- [x] Verify the fast checks pass.

### Task 3: Cover the audience split with unit tests

**Files:**
- Modify: `code/backend/DD.Tests.Unit/ServiceAuth/TokenFlowTests.cs`
- Create: `code/backend/DD.Tests.Unit/ServiceAuth/TokenServiceTests.cs`

- [ ] In `TokenFlowTests`, serialize the access token with `OAuthConstants.AccessTokenAudience` and validate it with `ValidAudience = OAuthConstants.AccessTokenAudience`, so the test mirrors `AuthService.CreateAccessTokenAsync`.
- [ ] Add a `TokenServiceTests` test asserting `Serialize(buildInfo, audience: OAuthConstants.AccessTokenAudience)` produces a JWT whose `aud` claim equals `dd-oauth-access`.
- [ ] Add a `TokenServiceTests` test asserting `Serialize(buildInfo)` with no audience produces a JWT whose `aud` claim equals the configured `Auth:Audience`.
- [ ] Verify the fast checks pass.

### Task 4: Verify the audience split against a live backend

**Files:**
- (no source changes — live verification of the running app)

- [ ] Ensure MongoDB is up (`./infra/up.sh`) and start `DD.App` on `:5000` as a detached background process (log to a file, capture the PID); wait until `curl -fsS http://localhost:5000/healthcheck` returns `Healthy`.
- [ ] Bootstrap credentials on the fresh database: `POST http://localhost:5000/api/test/CreateTestUser` returns JSON `{ "userId", "username", "password" }`; use the returned `username`/`password` in the steps below (there is no pre-seeded `test`/`test` user).
- [ ] Get a login token: `POST http://localhost:5000/api/auth/Account/SignIn` with `{"Username":"<username>","Password":"<password>"}` and capture the returned `token`. Confirm `GET "http://localhost:5000/api/task/Tasks?from=2000-01-01T00:00:00Z"` with header `Authorization: Bearer <token>` returns **200** (login token works on the app API) and `POST http://localhost:5000/mcp` with the same Bearer returns **401** (login token rejected at `/mcp`).
- [ ] Mint an MCP access token via the OAuth authorization_code + PKCE flow using the known test PKCE pair (verifier `dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk`, challenge `E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM`), a loopback `redirect_uri=http://127.0.0.1:5000/callback`, a fixed `client_id` (e.g. `smoke-client`) and the created user's creds: `POST http://localhost:5000/authorize` as form data `action=allow&username=<username>&password=<password>&client_id=smoke-client&redirect_uri=http://127.0.0.1:5000/callback&code_challenge=E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM&state=xyz`, read `code` from the `302` `Location` header, then `POST http://localhost:5000/token` as form data `grant_type=authorization_code&code=<code>&redirect_uri=http://127.0.0.1:5000/callback&client_id=smoke-client&code_verifier=dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk` and capture `access_token`.
- [ ] Confirm the MCP token is **rejected on the app API** — `GET "http://localhost:5000/api/task/Tasks?from=2000-01-01T00:00:00Z"` with `Authorization: Bearer <access_token>` returns **401** — and **accepted at `/mcp`** — `POST http://localhost:5000/mcp` with headers `Authorization: Bearer <access_token>`, `Content-Type: application/json`, `Accept: application/json, text/event-stream` and body `{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"smoke","version":"1"}}}` returns a non-401 success (200).
- [ ] Stop the `DD.App` process by PID (leave MongoDB running).

### Task 5: Final validation (full pass + local e2e)

**Files:**
- (no source changes — mandated final gate)

- [ ] Build the whole backend solution with no warnings: `dotnet build code/backend/DarkDeeds.sln -c Release`.
- [ ] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`.
- [ ] Run the full frontend gate: `cd code/frontend && npm run ci`.
- [ ] Bring up the full local stack: MongoDB via `./infra/up.sh`, backend `dotnet run --project code/backend/DD.App` on `:5000` (detached; wait for `curl -fsS http://localhost:5000/healthcheck` => `Healthy`), frontend `cd code/frontend && npm run dev` on `:3000` (detached; wait for HTTP 200). Start the servers as detached background processes, never piped to `head`/`tail`.
- [ ] Run the e2e suite against the local stack and confirm **ALL** tests pass: `dotnet test code/tests/DarkDeeds.E2eTests` (defaults `URL=http://localhost:3000` / `BE_URL=http://localhost:5000`; uses a local ChromeDriver, so Chrome must be installed).
- [ ] Fix any e2e failure and re-run until the whole e2e suite is green, then tear down the backend and frontend you started (stop each by PID; leave MongoDB running).

## Notes

- **No new configuration**: the `dd-oauth-access` audience is a compile-time invariant
  (like the internal `OAuthTokenKinds` authcode/refresh audiences), so no
  `appsettings`/bicep/dedicated-host changes are needed.
- **Constant placement**: `dd-oauth-access` goes in the public `OAuthConstants` because
  validation lives in `DD.ServiceAuth.Details` (which references `DD.ServiceAuth.Domain`);
  a public member cannot live in the internal `OAuthTokenKinds` class. A dedicated public
  class is an acceptable alternative.
- **Transition window**: MCP access tokens issued before deploy carry the old
  `Auth:Audience` and will be rejected at `/mcp` until the client refreshes; refresh
  tokens keep working and mint a new `dd-oauth-access` token. Access-token lifetime is
  60 min, so the window is small.
- **MCP surface**: the only MCP endpoint is `MapMcp("/mcp")`, wired in
  `code/backend/DD.Clients.Details/Setup.cs` (there is no separate `McpController`). The
  regular app endpoints, `/ws`, and this `/mcp` endpoint are the surfaces affected by the
  audience split.
- Everything committed is in English; no `// important`-marked code is modified.
