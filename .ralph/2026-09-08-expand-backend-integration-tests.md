# Plan: Expand Backend Integration Coverage

## Problem

The first integration-testing delivery established a shared Testcontainers MongoDB, an in-memory
`DD.App` host, real HTTP authentication, parallel isolation, and Auth/Tasks coverage. Recurrences,
Web BFF Settings, SignalR, Telegram, OAuth/MCP, and Mobile still rely mainly on mock-heavy
orchestration tests or the much slower Selenium suite, leaving real routing, persistence,
background notification, cache, and protocol composition insufficiently exercised.

Extend the integration layer across all remaining backend surfaces in one implementation run. Each
surface is tested through its public HTTP or protocol contract, with direct service access limited
to mobile setup that has no public provisioning API. Existing smoke/E2E tests remain unchanged.

## Approach

Reuse the existing single `mongo:4.4` Testcontainer, random database, shared
`WebApplicationFactory<Startup>`, `IntegrationEnvironmentLifetime`, real `TestUserClient`, and
parallel xUnit collections. Do not redesign the container lifecycle or change production behavior.
Add focused adapters only: LongPolling SignalR connections backed by fresh
`TestServer.CreateHandler` instances, a thread-safe recording Telegram boundary registered with
`RemoveAll<IBotSendMessageService>()` followed by `AddSingleton`, an official MCP client over the
factory-created `HttpClient`, and a scoped setup hook for mobile mapping.

Use unique usernames, date-only UTC task/recurrence values, UIDs, negative Telegram chat IDs, OAuth
state, and mobile keys. MongoDB stores millisecond precision, so recurrence no-op assertions never
use sub-millisecond values. Background notifications are globally FIFO through one hosted channel;
positive sentinel updates and bounded polling prove negative behavior without fixed sleeps. Every
SignalR connection and protocol session is asynchronously disposed before its test finishes.

Complete `.ralph/2026-09-08-upgrade-mongodb-driver.md` first. This plan assumes MongoDB.Driver
3.11.1 and a clean transitive vulnerability report are already present on its execution HEAD.

## Validation

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

## Todos

### Task 1: Cover Recurrences and remove replaced units

**Files:**
- Create: `code/backend/DD.Tests.Integration/RecurrencesIntegrationTests.cs`
- Create: `code/backend/DD.Tests.Integration/Helpers/RecurrencesHelper.cs`
- Delete: `code/backend/DD.Tests.Unit/ServiceTask/Services/RecurrenceServiceTests/RecurrenceServiceTest.LoadAsync.cs`
- Delete: `code/backend/DD.Tests.Unit/ServiceTask/Services/RecurrenceServiceTests/RecurrenceServiceTest.SaveAsync.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Services/RecurrenceCreatorServiceTests/RecurrenceCreatorServiceTest.CreateAsync.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Mocks/MocksCreator.Repo.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Mocks/MocksCreator.Spec.cs`

- [ ] Add authenticated HTTP coverage for empty load, create, unchanged no-op count, update, soft delete, UTC round-trip, weekday flags, and deleted-item exclusion through `/api/task/recurrences`, using only `DateTime.UtcNow.Date`-derived values.
- [ ] Add two-user isolation; assert a foreign recurrence UID returns HTTP 500 with ProblemDetails title `An unexpected error occurred`, and verify the owner's recurrence remains unchanged without modifying error handling.
- [ ] Seed `StartDate == EndDate == DateTime.UtcNow.Date` with `EveryNthDay=1`; prove concurrent and repeated `POST /api/task/recurrences/create?timezoneOffset=0` calls have result counts summing to one and Tasks HTTP contains exactly one generated task; add a no-schedule recurrence and assert it creates zero tasks.
- [ ] Delete all seven RecurrenceService interaction facts and delete only `CreateAsync_DoNothingIfNoNonDeletedRecurrences`, `CreateAsync_CreateTaskForRecurrence`, `CreateAsync_IgnoreAlreadyExistingRecurrences`, `CreateAsync_FilterPlannedRecurrencesByUser`, and `CreateAsync_NoRepeats` from the creator tests.
- [ ] Retain `CreateAsync_CreateRecurrenceTaskForRecurrence`, `CreateAsync_NotifyAboutCreatedTasks`, `CreateAsync_CreateEntitiesExpectedNumberOfTimes`, the four simple/combined schedule facts, and every EvaluatePeriod/MatchPeriod/MatchNthDay/MatchWeekday/MatchMonthDay matrix; remove only mock helpers proven unused afterward.
- [ ] Verify the fast checks pass with 0 warnings and leave the `// important` task predicate unchanged.

### Task 2: Cover Web BFF Settings

**Files:**
- Create: `code/backend/DD.Tests.Integration/SettingsIntegrationTests.cs`

- [ ] Assert a new authenticated user receives `showCompleted=false` from `GET /api/web/settings`.
- [ ] Save true and false through `POST /api/web/settings` and verify each persisted value through a later GET.
- [ ] Use two real users to prove settings isolation by authenticated user ID.
- [ ] Verify the fast checks pass with 0 warnings.

### Task 3: Cover SignalR notification contracts

**Files:**
- Modify: `code/backend/DD.Tests.Integration/DD.Tests.Integration.csproj`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/DarkDeedsWebApplicationFactory.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironment.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironmentLifetime.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationTestBase.cs`
- Create: `code/backend/DD.Tests.Integration/Helpers/SignalRHelper.cs`
- Create: `code/backend/DD.Tests.Integration/SignalRIntegrationTests.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Services/RecurrenceCreatorServiceTests/RecurrenceCreatorServiceTest.CreateAsync.cs`

- [ ] Add a direct `Microsoft.AspNetCore.SignalR.Client` reference and factory/environment methods that create authenticated hub clients for `/ws/task/task` through a fresh `TestServer.CreateHandler`, force `HttpTransportType.LongPolling`, and leave shared host configuration immutable.
- [ ] Add an `IAsyncDisposable` update collector that registers `update` before `StartAsync`, records arrival order by unique task UID, uses bounded waits and positive sentinel updates, and disposes every `HubConnection` with `await using`.
- [ ] Assert anonymous hub negotiation is rejected and a real login token starts an authenticated connection.
- [ ] Prove updates stay inside the authenticated user group, and prove `X-Client-Id` suppresses every matching logical client while a different client receives the target; use unsuppressed sentinel saves to complete all negative assertions without arbitrary sleeps.
- [ ] Generate a recurrence through the public create endpoint while its user's hub is connected, assert the generated task arrives through `update`, then delete only `CreateAsync_NotifyAboutCreatedTasks`; retain `CreateAsync_CreateRecurrenceTaskForRecurrence` because recurrence-to-task linkage is not publicly observable.
- [ ] Verify the fast checks pass with 0 warnings while retaining browser cross-tab smoke coverage for real WebSocket/Kestrel behavior.

### Task 4: Cover Telegram webhook behavior and remove replaced units

**Files:**
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/DarkDeedsWebApplicationFactory.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironment.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironmentLifetime.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationTestBase.cs`
- Create: `code/backend/DD.Tests.Integration/Infrastructure/RecordingBotSendMessageService.cs`
- Create: `code/backend/DD.Tests.Integration/Helpers/TelegramHelper.cs`
- Create: `code/backend/DD.Tests.Integration/TelegramIntegrationTests.cs`
- Delete: `code/backend/DD.Tests.Unit/TelegramClient/Services/BotProcessMessageServiceTest.cs`
- Delete: `code/backend/DD.Tests.Unit/TelegramClient/Services/CommandProcessor/StartCommandProcessorTest.cs`
- Delete: `code/backend/DD.Tests.Unit/TelegramClient/Services/CommandProcessor/ShowTodoCommandProcessorTest.cs`
- Modify: `code/backend/DD.Tests.Unit/TelegramClient/Services/CommandProcessor/CreateTaskCommandProcessorTest.cs`

- [ ] Add a shared scoped-execution hook, then register a thread-safe recorder with `services.RemoveAll<IBotSendMessageService>()` and `services.AddSingleton<IBotSendMessageService, RecordingBotSendMessageService>()`; assert from a created scope that the resolved service is the recorder.
- [ ] Assert anonymous start access is 401, then POST `api/tlgm/start?timezoneOffset=0`, parse the sole non-empty start key from `TelegramStartDto.Url`, POST `/start <key>` to `/api/tlgm/bot/integration-tests`, and observe `Registered` for a unique negative chat ID.
- [ ] POST a unique task command through the webhook, observe `Task created`, and verify persistence and two-user isolation through Tasks HTTP.
- [ ] Cover `/todo` with tasks, `/todo` with no tasks, and an unknown command by asserting exact recorded messages through bounded per-chat channels.
- [ ] Delete the complete BotProcess, StartCommand, and ShowTodo mock test files; delete only `CreateTaskCommandProcessorTest.ProcessAsync` and retain `ProcessAsync_WhenParseThrows_SendsFailedAndDoesNotSave`, parser/value tests, and `BaseCommandProcessor` failure policy.
- [ ] Verify the fast checks pass with 0 warnings and leave the existing Selenium Telegram test unchanged.

### Task 5: Cover OAuth discovery and token flow

**Files:**
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/DarkDeedsWebApplicationFactory.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironment.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironmentLifetime.cs`
- Create: `code/backend/DD.Tests.Integration/Infrastructure/OAuthMcpTestSession.cs`
- Create: `code/backend/DD.Tests.Integration/Helpers/OAuthMcpHelper.cs`
- Create: `code/backend/DD.Tests.Integration/OAuthIntegrationTests.cs`

- [ ] Expose `AuthIssuer` as an internal constant for assertions while leaving the normal shared-client base address unchanged; add a narrow client option that disables automatic redirects for OAuth authorization-contract requests.
- [ ] Add a disposable OAuth session that registers a fixed loopback callback on port 43123, computes the RFC 7636 S256 vector, submits authenticated allow consent, validates callback state, exchanges the form-encoded code, and owns login/access/refresh token state.
- [ ] Assert exact authorization-server and MCP protected-resource metadata, valid registration, the original GET authorization redirect status/location with auto-redirect disabled, deny redirect with preserved state, and unauthenticated allow rejection.
- [ ] Assert successful code exchange returns Bearer, scope `mcp`, expected expiry, non-empty access/refresh tokens, and `Cache-Control: no-store`; assert a wrong verifier returns HTTP 400 `invalid_grant`.
- [ ] Exchange the refresh token and assert another valid access token without testing replayable code/refresh, ignored scope, or ignored resource behavior.
- [ ] Verify the fast checks pass with 0 warnings.

### Task 6: Cover MCP tools and remove replaced units

**Files:**
- Modify: `code/backend/DD.Tests.Integration/DD.Tests.Integration.csproj`
- Create: `code/backend/DD.Tests.Integration/McpIntegrationTests.cs`
- Modify: `code/backend/DD.Tests.Integration/Infrastructure/OAuthMcpTestSession.cs`
- Modify: `code/backend/DD.Tests.Integration/Helpers/OAuthMcpHelper.cs`
- Delete: `code/backend/DD.Tests.Unit/ServiceAuth/OAuthFlowServiceTests.cs`
- Delete: `code/backend/DD.Tests.Unit/ServiceAuth/TokenFlowTests.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceAuth/AuthServiceTests.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceAuth/RefreshTokenServiceTests.cs`
- Modify: `code/backend/DD.Tests.Unit/McpClient/AddTasksToolTests.cs`
- Modify: `code/backend/DD.Tests.Unit/McpClient/UpdateTasksOrderToolTests.cs`
- Modify: `code/backend/DD.Tests.Unit/McpClient/McpServiceTests.cs`
- Delete: `code/backend/DD.Tests.Unit/ServiceTask/Services/TaskServiceTests/TaskServiceTest.LoadTasksByDateAsync.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Services/TaskServiceTests/TaskServiceTest.UpdateTasksAsync.cs`

- [ ] Add a direct `ModelContextProtocol` reference, then use official `HttpClientTransport` with `TransportMode=StreamableHttp`, bounded connection/call cancellation, and `McpClient` against `/mcp`; assert exactly `AddTasks`, `LoadTasks`, and `UpdateTasksOrder` are listed.
- [ ] Prove login, refresh, and authorization-code tokens cannot initialize MCP; prove OAuth access tokens initialize MCP but receive 401 from normal Tasks REST.
- [ ] Call `AddTasks` and `LoadTasks` with unique dated tasks, assert JSON-in-text fields and exclusive range filtering, and verify persisted state through login-authenticated Tasks HTTP.
- [ ] Call `UpdateTasksOrder` with valid and mixed unknown/deleted/foreign/stale updates, assert only successful updates and incremented versions, verify through REST, and observe the successful batch through SignalR with no client-id suppression.
- [ ] Delete OAuthFlowServiceTests, TokenFlowTests, `AuthServiceTests.CreateAccessTokenAsync_ExistingUser_SerializesWithMcpAudienceAndLifetime`, `RefreshTokenServiceTests.RefreshGrant_IssuesNewVerifiableToken`, both MCP tool valid-forwarding facts, all McpService mapping/forwarding assertions while retaining focused justification logging facts, and `TaskServiceTest.LoadTasksByDateAsync.cs`; from UpdateTasksAsync delete only SuccessfulUpdate, NonExistentTask, DeletedTask, ForeignTask, VersionConflict, and MultipleUpdatesWithMixedResults, retaining EmptyCollection, AllUpdatesFail, DoesNotNotifyOnNoSuccessfulUpdates, UpdatesOrderCorrectly, and NotifiesOnSuccess.
- [ ] Verify the fast checks pass with 0 warnings; if official `McpClient` initialization cannot complete within the bounded timeout over TestServer, implement the same bounded initialize/tools-list/tools-call contracts with raw JSON-RPC POSTs over the factory client rather than weakening assertions or starting a real network server.

### Task 7: Cover Mobile Watch and cache invalidation

**Files:**
- Create: `code/backend/DD.Tests.Integration/Helpers/MobileHelper.cs`
- Create: `code/backend/DD.Tests.Integration/WatchIntegrationTests.cs`

- [ ] Decode the `sub` claim from `TestUserClient.Token` and use the Task 4 scoped-execution hook with concrete `MobileUserRepository.UpsertAsync` only to seed the otherwise unprovisionable mapping with a unique mobile key.
- [ ] Create captured-UTC-today Routine, Simple, Additional, completed, and timed tasks through Tasks HTTP, then assert exact anonymous widget/app filtering, ordering, counts, formatted text, `main`, `support`, and `isSupport` values.
- [ ] After all setup writes, send a separate unmapped user's SignalR sentinel and wait for it to drain the shared FIFO notifier before warming either mobile cache.
- [ ] Warm both caches, update only the mapped user's task through HTTP, and poll widget/app routes with a bounded eventual assertion until both payloads change, proving the tested notification invalidated both entries.
- [ ] Assert an unknown mobile key returns HTTP 500 with ProblemDetails title `An unexpected error occurred` without changing production error handling; keep every pure `WatchPayloadControllerTests` case.
- [ ] Verify the fast checks pass with 0 warnings.

### Task 8: Final validation

**Files:**
- None (validation only)

- [ ] From the repository root, run `dotnet build code/backend/DarkDeeds.sln -c Release`, then `dotnet test code/backend/DarkDeeds.sln -c Release`; fix every failure, audit finding, and warning.
- [ ] Run `cd code/frontend && npm run ci`; fix every failure and warning, then return to the repository root.
- [ ] Store owned server PIDs in `/tmp/dd-ralph-backend.pid` and `/tmp/dd-ralph-frontend.pid`; before startup, if port 5000 or 3000 is occupied, kill only a still-running process whose matching PID file, command, and port ownership identify the backend project or frontend Vite process, then re-check; abort naming the occupied port if no owned process can be proven.
- [ ] Run `./infra/up.sh`, verify MongoDB and Selenium Grid on ports 27017/4444, start `dotnet run --project code/backend/DD.App` and `cd code/frontend && npm run dev` as detached processes with separate logs and PID files, and poll with deadlines until backend health is `Healthy` and frontend returns HTTP 200.
- [ ] Run `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests` without piping; require `Failed: 0, Passed: 11, Skipped: 1`; after any code fix, stop the owned servers, rerun affected backend/frontend gates, restart both servers, wait for readiness, and rerun the entire E2E suite.
- [ ] Kill only recorded backend/frontend PIDs, verify ports 5000/3000 are free, leave MongoDB/Grid running, and confirm no `dd-integration-tests-*` container remains after testhost exit.

## Notes

- Drafted against clean `master` HEAD `431a140c58767d38f95a1c46563840af8b3dce12`, including merged foundation PR #381. Revalidate this plan and every referenced contract if HEAD changes before Ralph executes it.
- This plan must run only after `.ralph/2026-09-08-upgrade-mongodb-driver.md` completes and its changes are present on the current HEAD; revalidate again after that prerequisite changes HEAD.
- Two adversarial review passes were completed before the dependency remediation was split into its own prerequisite plan.
- Existing Testcontainers lifecycle remains unchanged except narrow hub, client-option, service-scope, and recording accessors; one Mongo container and one application factory remain.
- SignalR uses LongPolling through TestServer; browser smoke retains real WebSocket/Kestrel responsibility.
- The shared notifier background service has one FIFO channel reader across all parallel tests; unique UIDs and sentinel ordering are required, and one slow subscriber can delay other asynchronous assertions.
- Existing Selenium tests, including Telegram, remain unchanged.
- OAuth replay/scope/resource limitations are neither fixed nor cemented by this plan.
- If official MCP SDK initialization times out over TestServer, bounded raw JSON-RPC over the same factory `HttpClient` is the required fallback; do not start Kestrel or bypass authentication.
- All repository content must remain English, and code marked `// important` must not be modified.
