# Plan: Add Terminal Client

## Problem

Dark Deeds has web, mobile-facing, Telegram, and MCP clients but no keyboard-first terminal client. The terminal client must expose the Overview workflow while remaining useful offline, persist every accepted edit before network synchronization, replay unsaved edits after restart, receive real-time updates, and resolve optimistic-concurrency conflicts with the same backend-wins semantics as the React frontend.

Spectre.Console supplies alternate-screen rendering, styling, and testable console I/O, but not a complete scrollable TUI framework. The client therefore needs an explicit application state machine, one state/render owner, a spatial focus graph, an integrated line editor, resize handling, and a custom viewport.

## Approach

Add a pure shared task-text parser plus three terminal projects:

- `DD.Shared.TaskText`: dependency-light parser/range-expansion core reused by the existing backend adapter and the terminal client.
- `DD.TerminalClient.Domain`: terminal models, local-date abstraction, mutations, synchronization, Overview projection, navigation, input modes, and application state transitions. It has no Spectre.Console, HTTP, SignalR, or file-system dependency.
- `DD.TerminalClient.Details`: profile/state/token storage, HTTP, SignalR, logging, Spectre rendering, viewport, and key input adapters.
- `DD.TerminalClient`: executable composition root, option handling, event loop, lifecycle, and unattended self-test entry point.

Use one single-reader `Channel<ApplicationEvent>` as the only writer of application and UI state. Keyboard, HTTP, SignalR, retry, renewal, resize, and cancellation producers may only enqueue events. The loop applies state transitions, starts effects, and refreshes the Spectre frame once per drained event batch. Apply each local task mutation to an atomic cache/outbox write before emitting its network-save effect.

Use injected `ILocalDateProvider` in Domain. The Details implementation derives the local calendar date from `TimeProvider.System.GetUtcNow()` and `TimeZoneInfo.Local`; do not use banned `DateTime.Now` or `DateTimeOffset.Now`. Represent task dates as `DateOnly?`, and map transport dates by UTC calendar components so SSH/client timezone changes do not shift dates.

### Existing server contract

Use the same lower-case paths as the frontend:

- `POST api/auth/account/signin`, body `{ username, password }`, response `{ token, result }`.
- `POST api/auth/account/renew`, bearer-authenticated, response plain JWT text.
- `GET api/task/tasks?from=<ISO UTC datetime>`.
- `POST api/task/tasks`, bearer-authenticated JSON task array.
- Hub `/ws/task/task?clientId=<uuid>` with server events `update` and `heartbeat`.
- REST requests send `Authorization: Bearer <jwt>` and `X-Client-Id: <same process uuid>`.

Task JSON fields are `uid`, `title`, `date`, `time`, `order`, `completed`, `deleted`, `type`, `isProbable`, and `version`. Load with `from` equal to the current local Monday mapped to UTC midnight. The unchanged `// important` server filter returns unfinished Simple tasks even before `from`, every No Date task, and tasks on/after `from`; these pre-Monday unfinished Simple tasks form Overdue. Do not modify that filter.

The server excludes the origin client's hub connections from save notifications. The terminal must apply its own REST save response immediately and must never wait for a hub echo to update versions or finish a save.

### Seeded profiles

Seed these named profiles while allowing user-created profiles:

```text
production = https://dark-deeds.com/
test = https://test.dark-deeds.com/
local = http://localhost:5000/
```

Require HTTPS except for loopback hosts. Isolate token, cache, outbox, settings, and logs by profile and data owner.

### Startup order

1. Parse `--profile`, `--help`, `--version`, `--self-test`, and optional state-root override.
2. For interactive mode, reject redirected/noninteractive input with a plain actionable message; `--help`, `--version`, and `--self-test` remain noninteractive.
3. Resolve or create the profile, then load token and atomic local state.
4. Validate JWT shape/expiry; if absent or unusable, enter masked login mode and persist only the returned token.
5. Enforce the data-owner guard: keep cache/outbox for the same user; require confirmation and clear profile user state for a different user.
6. Hydrate cached tasks, restore the durable outbox, and render immediately.
7. Enter hub buffering mode and start the hub.
8. Request the full task snapshot from current Monday. If offline, keep cached state and schedule retry without blocking the UI.
9. Reconcile the snapshot, replay buffered hub updates, then leave buffering mode.
10. Drain the outbox, start token-renewal and terminal-size timers, and continue the event loop.
11. On hub reconnect, repeat steps 7-9.
12. On `401`, stop network activity, clear only in-memory pending/in-flight state, preserve persisted outbox, and return to login; same-user login restores/replays it.

### Keymap and modes

Normal mode only:

| Keys | Action |
| --- | --- |
| Arrows, `h/j/k/l` | Spatial task navigation |
| `a` | Add using the focused task's date; if there is no focus, create No Date |
| `A` | Add No Date |
| `e` | Edit focused task |
| Space | Complete/uncomplete focused task |
| `d` | Enter delete confirmation |
| Shift+Up/Down, `K/J` | Reorder relative to visible neighbors while preserving hidden-task relative order |
| Shift+Left/Right, `H/L` | Move one calendar day; unavailable for No Date until an explicit date is chosen |
| `m` | Open editor for explicit date/No Date move |
| `r` | Toggle Routine visibility for the focused dated day; otherwise show a status message |
| `c` | Toggle completed visibility locally |
| `Ctrl+R` | Force hub reconnect and full snapshot reload |
| `?` | Toggle help |
| `q` | Quit |

Editor/login mode treats printable keys as text and reserves only cursor/editing keys, Enter, and Escape. Delete-confirmation mode accepts `y`, `n`, or Escape. Help mode closes with `?` or Escape. Resize-required mode continues synchronization and accepts `?` and `q`.

Use `IAnsiConsole.Input.ReadKeyAsync(intercept: true, cancellationToken)` behind `IKeyInputSource`; all input/application tests use queued `TestConsole` keys and explicit timeouts.

### Distribution and exclusions

Publish untrimmed self-contained single-file binaries for `osx-arm64`, `osx-x64`, `linux-x64`, and `linux-arm64`. A separate `workflow_dispatch` and `v*` tag workflow calls a locally testable publish script. Windows, mouse input, recurrences/settings screens, interactive conflict merge, Homebrew, signing, and notarization are outside the first release. Ghostty/SSH visual checks remain a documented human checklist, not an unattended Ralph gate.

## Validation

```bash
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

Every task must finish with both commands passing and zero warnings. This plan does not modify `code/frontend/**`; if implementation expands into that directory, also run `cd code/frontend && npm run ci` after each affected task.

## Todos

### Task 1: Scaffold shared and terminal projects

**Files:**
- Modify: `code/backend/DarkDeeds.sln`
- Modify: `code/backend/Directory.Packages.props`
- Modify: `code/backend/DD.ServiceTask.Domain/DD.ServiceTask.Domain.csproj`
- Modify: `code/backend/DD.Tests.Unit/DD.Tests.Unit.csproj`
- Create: `code/backend/DD.Shared.TaskText/DD.Shared.TaskText.csproj`
- Create: `code/backend/DD.TerminalClient.Domain/DD.TerminalClient.Domain.csproj`
- Create: `code/backend/DD.TerminalClient.Details/DD.TerminalClient.Details.csproj`
- Create: `code/backend/DD.TerminalClient/DD.TerminalClient.csproj`
- Create: `code/backend/DD.TerminalClient/Program.cs`

- [x] Add `DD.Shared.TaskText` under the solution `Shared` folder and the three terminal projects under `Clients`; keep the standalone Details project intentionally separate from the server-hosted `DD.Clients.Details` assembly.
- [x] Set `DD.TerminalClient` to `<OutputType>Exe</OutputType>` and establish references `TerminalClient -> Domain + Details`, `Details -> Domain + Shared.Details.Abstractions`, `Domain -> Shared.TaskText + Shared.Details.Abstractions`, and `ServiceTask.Domain -> Shared.TaskText`; prevent reverse or UI/infrastructure dependencies into Domain.
- [x] Add project references from `DD.Tests.Unit` to `DD.Shared.TaskText`, `DD.TerminalClient.Domain`, `DD.TerminalClient.Details`, and `DD.TerminalClient`.
- [x] Pin `Spectre.Console` and `Spectre.Console.Testing` to `0.57.2`, `Microsoft.AspNetCore.SignalR.Client` to `8.0.11`, and `Microsoft.Extensions.Hosting` to `9.0.0`; reuse existing central `Microsoft.Extensions.Http`, logging, and JSON versions.
- [x] Add `--help` and `--version` plain-output paths and a non-TTY guard that never enters alternate screen when input/output is redirected; run Release restore/build without suppressing NuGet audit findings, and move an audited package only to the lowest clean patch in the same major line.
- [x] Verify the fast checks pass with zero warnings.

### Task 2: Extract the shared task-text parser

**Files:**
- Create: `code/backend/DD.Shared.TaskText/ParsedTaskText.cs`
- Create: `code/backend/DD.Shared.TaskText/ITaskTextDateProvider.cs`
- Create: `code/backend/DD.Shared.TaskText/TaskTextParser.cs`
- Modify: `code/backend/DD.ServiceTask.Domain/Services/TaskParserService.cs`
- Modify: `code/backend/DD.ServiceTask.Domain/Setup.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Services/TaskParserServiceTest.cs`
- Create: `code/backend/DD.Tests.Unit/Shared/TaskTextParserTests.cs`

- [x] Extract date, range, relative-date, time, flag, 2-31 day validation, and expansion behavior into a BCL-only parser returning `ParsedTaskText` values without `TaskDto`, logging, DI, or service-domain dependencies.
- [x] Keep `TaskParserService` as a thin adapter from shared parse results to `TaskDto`, retaining its public interface and existing consumers without changing `// important` code.
- [x] Move or duplicate assertions so every existing test in `TaskParserServiceTest.cs` still passes and every numbered parse/range case mirrored by `code/frontend/tests/services/TaskConvertService.test.ts` and `TaskRangeService.test.ts` has an equivalent shared-parser or adapter test.
- [x] Add explicit tests for invalid calendar dates, reversed/single-day/over-31-day ranges, mixed-year endpoints, duplicate/conflicting flags, and injected current date.
- [x] Register the shared parser through the existing service-domain setup and remove the old duplicated parsing implementation.
- [x] Verify the fast checks pass with zero warnings.

### Task 3: Add terminal task models, local date, formatter, and mutations

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Models/TerminalTask.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Models/TerminalTaskType.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Time/ILocalDateProvider.cs`
- Create: `code/backend/DD.TerminalClient.Details/Time/SystemLocalDateProvider.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Editing/TaskTextFormatter.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Tasks/TaskMutationService.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Tasks/TaskOrderService.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TaskMutationServiceTests.cs`

- [x] Define the terminal task model with `DateOnly?`, time minutes, task type, probable/completed/deleted flags, order, UID, and version; define persisted outbox items as terminal tasks keyed by UID.
- [x] Implement `ILocalDateProvider` and a Details adapter based on `TimeProvider.GetUtcNow()` plus `TimeZoneInfo.Local`, with no banned wall-clock APIs in Domain.
- [x] Implement formatting that round-trips shared parser results and terminal tasks into the web-compatible editor string, including single-date edit behavior.
- [x] Implement pure create, edit, complete/uncomplete, soft-delete, explicit move, one-day move, and visible-neighbor reorder commands.
- [x] Port every test in `code/frontend/tests/services/TaskSaveService.test.ts` to xUnit, including version preservation and contiguous renumbering of all changed tasks in source/destination groups; add hidden-neighbor and No Date cases.
- [x] Verify the fast checks pass with zero warnings.

### Task 4: Implement profile configuration and paths

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Profiles/TerminalProfile.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/IProfileStore.cs`
- Create: `code/backend/DD.TerminalClient.Details/Storage/ApplicationPathProvider.cs`
- Create: `code/backend/DD.TerminalClient.Details/Storage/ProfileStore.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/ProfileStoreTests.cs`

- [x] Implement macOS Application Support and Linux XDG config/state roots with an explicit override used by tests and self-test.
- [x] Seed production, test, and local profiles with the exact base URLs in Approach, while supporting user-created named profiles and `--profile` selection.
- [x] Isolate every profile directory and validate profile names against traversal/invalid filename characters.
- [x] Require HTTPS except for loopback hosts and normalize base URIs to one trailing slash.
- [x] Test seeded profiles, custom profiles, path isolation, root override, invalid names, loopback HTTP, rejected remote HTTP, and URI normalization.
- [x] Verify the fast checks pass with zero warnings.

### Task 5: Implement atomic state, token storage, and file logging

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/State/PersistedTerminalState.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/ILocalStateStore.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/ITokenStore.cs`
- Create: `code/backend/DD.TerminalClient.Details/Storage/LocalStateStore.cs`
- Create: `code/backend/DD.TerminalClient.Details/Storage/TokenStore.cs`
- Create: `code/backend/DD.TerminalClient.Details/Logging/TerminalFileLoggerProvider.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/LocalStateStoreTests.cs`

- [x] Define a schema-versioned state containing data owner, cached tasks, durable outbox, and local completed visibility; add an explicit migration pipeline that never drops an outbox during upgrades.
- [x] Implement temporary write, flush-to-disk, and atomic replace while preserving the previous file on failure; malformed or unsupported state must be retained and surfaced as a blocking actionable error.
- [x] Store JWT separately, create/tighten token and user-state files to Unix mode `0600`, and keep passwords/tokens/authorization headers out of logs and errors.
- [x] Implement bounded per-profile file logging with no console provider during interactive mode.
- [x] Test first load, round trip, atomic replacement failure, malformed JSON, unsupported/current/older schema versions, migration with outbox preservation, permissions, and secret redaction.
- [x] Verify the fast checks pass with zero warnings.

### Task 6: Implement authentication and REST task clients

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/IAuthApiClient.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/ITaskApiClient.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Authentication/AuthSession.cs`
- Create: `code/backend/DD.TerminalClient.Details/Api/AuthApiClient.cs`
- Create: `code/backend/DD.TerminalClient.Details/Api/TaskApiClient.cs`
- Create: `code/backend/DD.TerminalClient.Details/Api/TerminalHttpHandler.cs`
- Create: `code/backend/DD.TerminalClient.Details/Api/TaskTransportMapper.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/HttpClientTests.cs`

- [x] Implement the exact sign-in, renew, task-load, and task-save routes/response media types from Approach with private auth DTOs and the shared `TaskDto` transport contract.
- [x] Send bearer auth and the process client ID, use finite timeouts/cancellation, and classify unauthorized, transport, validation, and unexpected protocol failures without broad catches.
- [x] Parse JWT username/expiry only for local session UX; never treat parsing as server validation and never log secrets.
- [x] Map `DateOnly` to UTC midnight and server UTC date components back to `DateOnly`; load from current local Monday mapped to UTC midnight.
- [x] Add fake-handler tests for exact routes, query, casing, media types, task JSON fields, headers, save response application data, mapping, timeouts, cancellation, and all error classes.
- [x] Verify the fast checks pass with zero warnings.

### Task 7: Implement SignalR realtime client

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/ITaskHubClient.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Realtime/TaskHubEvent.cs`
- Create: `code/backend/DD.TerminalClient.Details/Realtime/TaskHubClient.cs`
- Create: `code/backend/DD.TerminalClient.Details/Realtime/TerminalRetryPolicy.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TaskHubClientTests.cs`

- [x] Connect to the exact hub path with current JWT and the same process client ID used by REST; translate `update`, `heartbeat`, reconnecting, reconnected, closed, and unauthorized conditions into domain events only.
- [x] Implement 1, 2, 4, 8, 16, then 30-second reconnect delays, explicit start/stop cancellation, and token-provider refresh without mutating app/UI state from callbacks.
- [x] Implement ordered buffering entered before initial/reconnect snapshot loads and replayed only after reconciliation.
- [x] Route diagnostics only to the secret-safe file logger.
- [x] Test exact URL/query/event names, retry sequence, event translation, buffering order, cancellation, and refreshed token reads behind a testable connection abstraction.
- [x] Verify the fast checks pass with zero warnings.

### Task 8: Implement outbox and save state machine

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Synchronization/TaskSyncState.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Synchronization/TaskSyncCoordinator.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Synchronization/TaskSyncEffect.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TaskSyncSaveTests.cs`

- [x] Implement pending and in-flight maps where pending re-edits win by UID and only one batch may be in flight.
- [x] Emit atomic cache/outbox persistence before any save effect for every accepted mutation, and restore the persisted outbox on startup.
- [x] On transport failure, requeue non-superseded in-flight tasks, leave durable outbox content intact, emit status, and schedule exactly a five-second retry.
- [x] On success, apply the REST response immediately, propagate returned versions into newer pending edits, clear completed in-flight entries, and never wait for an origin-suppressed hub echo.
- [x] Port every test in `code/frontend/tests/services/TaskSyncService.test.ts` covering enqueue/save/retry/re-edit/reset/outbox/version behavior, and add an explicit no-own-hub-echo test.
- [x] Verify the fast checks pass with zero warnings.

### Task 9: Implement conflicts and snapshot reconciliation

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Synchronization/TaskReconciler.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Synchronization/ReconciliationResult.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TaskReconcilerTests.cs`

- [x] Apply incoming tasks with no local pending edit and suppress incoming same/older versions while a local edit is pending.
- [x] When an incoming version is newer, drop pending/in-flight copies, apply the server task, persist the reduced outbox, and emit a conflict notification containing task identity/title.
- [x] Reconcile full snapshots by processing the same conflict rule, removing cached UIDs absent from both snapshot and pending state, and retaining soft-delete records only as supplied/pending.
- [x] Replay buffered hub updates after each successful snapshot and preserve arrival order; on failed snapshot keep the buffer and cached state for retry.
- [x] Port every online-update/reconcile case from frontend sync tests and add stale deletion, buffered duplicate version, and reconnect ordering tests.
- [x] Verify the fast checks pass with zero warnings.

### Task 10: Build Overview projection

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Overview/OverviewProjectionService.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Overview/OverviewProjection.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Overview/VisualTaskAddress.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/OverviewProjectionTests.cs`

- [x] Project full-width No Date, conditional dated Overdue, Current with exactly 14 Monday-based cells in two rows of seven, and Future rows containing only nonempty dates.
- [x] Define Overdue as returned unfinished Simple dated tasks before current Monday, without changing the `// important` server filter or inventing unavailable old task types.
- [x] Filter deleted tasks, optionally filter completed tasks, collapse Routine tasks per date, sort by order, and include stable section/row/column/task-index addresses consumed by navigation and rendering.
- [x] Preserve empty Current day cells in layout while excluding them from focus; wrap Future at seven visible date columns.
- [x] Port every test in `code/frontend/tests/services/OverviewService.test.ts` and add timezone/local-Monday, Overdue contract, Routine collapse, completed toggle, and all-empty cases.
- [x] Verify the fast checks pass with zero warnings.

### Task 11: Implement spatial navigation and focus fallback

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Navigation/TaskNavigationService.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Navigation/TaskFocusService.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TaskNavigationServiceTests.cs`

- [x] Move Up/Down within a task list, then across previous/next visual rows preferring the same column and clamping task index to the nearest available visible task.
- [x] Move Left/Right to the nearest nonempty dated cell in the same visual row without wrapping, preserving approximate task index and skipping empty Current cells.
- [x] Implement No Date/Overdue section transitions, initial focus priority, no-visible-task state, UID preservation, and nearest-old-address fallback.
- [x] Recompute focus after delete, completed filtering, Routine collapse, explicit move, snapshot removal, and server-wins conflict.
- [x] Test uneven columns, empty days, both Current rows, wrapped Future rows, every section boundary, filtered selected tasks, and remote-removal fallback.
- [x] Verify the fast checks pass with zero warnings.

### Task 12: Render the Spectre terminal frame

**Files:**
- Create: `code/backend/DD.TerminalClient.Details/Ui/OverviewRenderer.cs`
- Create: `code/backend/DD.TerminalClient.Details/Ui/TerminalStyles.cs`
- Create: `code/backend/DD.TerminalClient.Details/Ui/TerminalFrame.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/OverviewRendererTests.cs`

- [ ] Render fixed header/footer plus Overview content using Spectre `Grid`, `Panel`, and `Text`, with escaped user content rather than raw markup.
- [ ] Style selected, today, completed, probable, Additional, Routine, Weekly, and timed tasks; show collapsed Routine count and full selected text in the footer.
- [ ] Render each task as one ellipsized line and produce deterministic line metadata for task addresses without relying on wrapped titles.
- [ ] Render normal, editor, login, confirmation, help, offline, conflict, and empty-state status content without running prompts inside the live display.
- [ ] Add TestConsole assertions for empty, normal, dense, long, markup-like, Unicode, each task style, and each UI mode.
- [ ] Verify the fast checks pass with zero warnings.

### Task 13: Implement viewport and resize behavior

**Files:**
- Create: `code/backend/DD.TerminalClient.Details/Ui/ViewportRenderable.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Overview/ViewportState.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/ViewportRenderableTests.cs`

- [ ] Clip rendered Spectre segments by line while preserving styles, reserve fixed header/footer height, and expose top/bottom continuation indicators.
- [ ] Adjust viewport offset after focus/layout changes so the focused task's complete line remains visible without independent non-task scrolling.
- [ ] Recompute layout and line metadata on dimension changes and tolerate unavailable window-size APIs by falling back to Spectre profile capabilities.
- [ ] Below 120x30 render resize-required mode while synchronization continues and `?`/`q` remain accepted.
- [ ] Test top/middle/bottom clipping, style preservation, focus following, resize transitions, unsupported size APIs, and content shorter than viewport.
- [ ] Verify the fast checks pass with zero warnings.

### Task 14: Implement input modes, editor, and keymap

**Files:**
- Create: `code/backend/DD.TerminalClient.Domain/Input/TerminalUiMode.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Input/TerminalCommand.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Editing/LineEditorState.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Input/TerminalInputReducer.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Abstractions/IKeyInputSource.cs`
- Create: `code/backend/DD.TerminalClient.Details/Ui/TerminalInputReader.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TerminalInputTests.cs`

- [ ] Implement the exact keymap and allowed modes from Approach, including no-op/status behavior for commands without valid focus/date context.
- [ ] Implement normal, editor, masked login, delete-confirmation, help, and resize-required reducers so text input cannot trigger normal commands.
- [ ] Implement printable insertion/paste, cursor movement, Backspace/Delete, Home/End, live shared-parser feedback, Enter commit, and Escape cancel.
- [ ] Use cancellable Spectre `ReadKeyAsync` behind `IKeyInputSource`; TestConsole key queues and explicit test timeouts must prevent hung test runs.
- [ ] Test every listed key/alias, mode transition, confirmation, invalid context, editor operation, masked value, parse error, commit, cancel, and cancellation.
- [ ] Verify the fast checks pass with zero warnings.

### Task 15: Wire event loop, startup, and lifecycle

**Files:**
- Modify: `code/backend/DD.TerminalClient/Program.cs`
- Create: `code/backend/DD.TerminalClient/TerminalApplication.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Application/ApplicationEvent.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Application/ApplicationState.cs`
- Create: `code/backend/DD.TerminalClient.Domain/Application/ApplicationReducer.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TerminalApplicationTests.cs`

- [ ] Register exact adapters and create one single-reader event channel; all input/network/timer callbacks only enqueue events, and every app test has a timeout.
- [ ] Implement the 12-step startup/reconnect/401 sequence from Approach, including offline cached startup, profile owner guard, buffered snapshot, outbox restore/drain, and token renewal below one day.
- [ ] Run alternate screen and live refresh under the event-loop owner, coalesce rendering after drained batches, and keep file logging/prompts from writing to the interactive console.
- [ ] Implement cancellation and fatal-error flow that persists current state, leaves unsaved tasks in outbox, stops hub/timers, and restores cursor/screen in `finally`.
- [ ] Add fake-console/clock/storage/API/hub tests for startup success, offline startup, reconnect, same/different user, 401, renewal, retry, conflict, resize, quit, and fatal cleanup.
- [ ] Verify the fast checks pass with zero warnings.

### Task 16: Add unattended real-backend self-test

**Files:**
- Modify: `code/backend/DD.TerminalClient/Program.cs`
- Create: `code/backend/DD.TerminalClient/SelfTest/TerminalSelfTest.cs`
- Create: `code/backend/DD.Tests.Unit/TerminalClient/TerminalSelfTestTests.cs`

- [ ] Add `--self-test` that never enters alternate screen, requires `DD_TERMINAL_USERNAME` and `DD_TERMINAL_PASSWORD`, accepts the normal `--profile` plus state-root override, and returns nonzero with secret-safe diagnostics on any failed assertion.
- [ ] Against the selected real backend, sign in, open writer and observer hub connections with distinct client IDs, load the snapshot, and wait with finite timeouts.
- [ ] Create a uniquely named No Date task through the writer, assert REST version assignment and observer `update`, update/complete it and assert version increment plus observer update, then soft-delete it and assert observer deletion update.
- [ ] Use a temporary state root, clean the created task in `finally`, stop both hubs, and never print credentials/JWT; leave enough diagnostics to identify the failed contract stage.
- [ ] Unit-test success, timeout, cleanup-after-failure, missing environment, redaction, and exit codes with fake adapters.
- [ ] Verify the fast checks pass with zero warnings.

### Task 17: Add publish script, release workflow, and documentation

**Files:**
- Modify: `code/backend/DD.TerminalClient/DD.TerminalClient.csproj`
- Create: `scripts/publish-terminal-client.sh`
- Create: `.github/workflows/terminal-client-release.yml`
- Create: `code/backend/DD.TerminalClient/README.md`

- [ ] Configure untrimmed self-contained single-file publish and a tested script that builds `osx-arm64`, `osx-x64`, `linux-x64`, and `linux-arm64`, sets executable archive contents, creates deterministic archives/checksums, and accepts a concrete output-directory argument.
- [ ] Make the release workflow standalone from `ci.yml`, triggered only by `workflow_dispatch` and `v*` tags, and have it invoke the same publish script before uploading artifacts.
- [ ] Smoke-run host-compatible output with `--version`, run `bash -n scripts/publish-terminal-client.sh`, and keep all workflow/script names and output English.
- [ ] Document installation, profiles, login/token behavior, shortcuts, local state/reset/migration, conflict rules, local backend use, self-test environment, and troubleshooting.
- [ ] Add a clearly labeled human-only checklist for Ghostty local rendering, SSH PTY allocation, remote `xterm-ghostty` terminfo, resize, and alternate-screen restoration; do not include it in unattended completion criteria.
- [ ] Verify the fast checks pass with zero warnings.

### Task 18: Final validation

**Files:**
- Modify only files required to fix failures found by these gates.

- [ ] Run `dotnet build code/backend/DarkDeeds.sln -c Release` and fix every error and warning.
- [ ] Run `dotnet test code/backend/DarkDeeds.sln -c Release` and fix every failure.
- [ ] Run `cd code/frontend && npm run ci` and fix every failure.
- [ ] Create a concrete temporary publish directory, run the publish script, verify all four archives/checksums exist, and smoke-run every host-compatible binary:

  ```bash
  PUBLISH_DIR=$(mktemp -d)
  scripts/publish-terminal-client.sh "$PUBLISH_DIR"
  find "$PUBLISH_DIR" -maxdepth 1 -type f -print
  ```

- [ ] Run `./infra/up.sh`; start `dotnet run --project code/backend/DD.App` on port 5000 and `cd code/frontend && npm run dev` on port 3000 as detached processes with recorded PIDs; wait for backend `Healthy` and frontend HTTP 200; create a fresh local test user and run the terminal self-test with a concrete temporary state root, iterating until exit code 0:

  ```bash
  STATE_ROOT=$(mktemp -d)
  CREDS=$(curl -fsS -X POST http://localhost:5000/api/test/CreateTestUser)
  USERNAME=$(node -e 'const x=JSON.parse(process.argv[1]); process.stdout.write(x.username ?? x.Username)' "$CREDS")
  PASSWORD=$(node -e 'const x=JSON.parse(process.argv[1]); process.stdout.write(x.password ?? x.Password)' "$CREDS")
  DD_TERMINAL_USERNAME="$USERNAME" DD_TERMINAL_PASSWORD="$PASSWORD" \
    dotnet run --project code/backend/DD.TerminalClient -- \
    --profile local --self-test --state-root "$STATE_ROOT"
  ```

- [ ] Run the Selenium Grid suite and iterate until `Failed: 0, Passed: 11, Skipped: 1`; then stop the backend and frontend by their recorded PIDs and leave MongoDB/Selenium Grid running:

  ```bash
  CONTAINER=true \
  SELENIUM_GRID_URL=http://localhost:4444 \
  URL=http://host.docker.internal:3000 \
  BE_URL=http://localhost:5000 \
  dotnet test code/tests/DarkDeeds.E2eTests
  ```

## Notes

- Do not modify `code/backend/DD.ServiceTask.Domain/Specifications/TaskSpecification.cs` because `FilterActual` is marked `// important`; its existing behavior is the Overdue contract.
- Do not modify any other `// important` code without explicit approval.
- Everything committed must be in English.
- Keep `PublishTrimmed` disabled for the first release.
- Backend contract changes are allowed only when the real `--self-test` proves a defect that cannot be fixed client-side.
- Ghostty/SSH visual verification is intentionally human-run and documented, not a Ralph completion gate.
