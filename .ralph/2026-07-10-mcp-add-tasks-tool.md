# Plan: Add an MCP tool to create new tasks (AddTasks)

## Problem

The Dark Deeds MCP server currently exposes two tools (`LoadTasks`, `UpdateTasksOrder`). An AI
agent connected over `/mcp` can read and reorder a user's tasks but **cannot create** them. We want
a new MCP tool, `AddTasks`, that lets the agent add brand-new tasks to the authenticated user's
Dark Deeds account.

The change is backend-only: new tasks reach the web client through the existing SignalR
`TaskUpdated` broadcast, so no frontend work is required.

## Approach

Follow the exact same layering/pattern as the existing MCP tools:

- **Tool** (`DD.Clients.Details/McpClient/Tools/`): a thin static `[McpServerTool]` method that
  resolves `userId` from `IUserAuth` and delegates to `IMcpService`.
- **Domain service** (`DD.McpClient.Domain/McpService.cs`): validates, maps input DTOs to `TaskDto`,
  persists, and serializes — using the existing `ITaskServiceApp`.
- **Persistence**: reuse `ITaskServiceApp.SaveTasksAsync(tasks, userId, clientId: null)` — the same
  call the Telegram bot's `CreateTaskCommandProcessor` uses to create tasks. Passing
  `clientId: null` triggers the SignalR `TaskUpdated` broadcast, so the web UI updates in real time
  with **no frontend changes**.

### Design decisions (confirmed with the user)

1. **Input = structured fields** (not text-DSL parsing): `Title`, `Date`, `Time`, `Type`,
   `IsProbable`. Consistent with `LoadTasks` output and `UpdateTasksOrder` input; predictable for an
   agent that already sees structured task JSON.
2. **Batch** — the tool accepts an **array** of tasks per call (like `SaveTasksAsync` /
   `UpdateTasksOrder`), yielding a single SignalR broadcast for the batch.
3. **Requires a `justification`** guardrail parameter, mirroring `UpdateTasksOrder`.
4. **Annotate all DTOs** in `DD.Shared.Details.Abstractions/Dto/` with `[Description]` for
   consistency (the user asked for uniformity, not just the new input DTO).

### New input DTO — `TaskCreateDto`

Add next to `TaskUpdateDto`. It exposes only the fields the agent may set (the service owns the
rest: `Uid`, `Order`, `Completed`, `Deleted`, `Version`):

| Field        | Type          | Notes                                                     |
| ------------ | ------------- | --------------------------------------------------------- |
| `Title`      | `string`      | Required, non-empty.                                      |
| `Date`       | `DateTime?`   | Optional (tasks may be dateless).                         |
| `Time`       | `int?`        | Optional, **minutes from midnight** (17:30 => 1050).      |
| `Type`       | `TaskTypeDto` | `Simple` (default) / `Additional` / `Routine` / `Weekly`. |
| `IsProbable` | `bool`        | Defaults to `false`.                                      |

Every property carries a `[Description]` attribute so the MCP-generated JSON schema guides the agent
(especially `Time` = minutes-from-midnight and the `Type` enum values).

### Mapping (`TaskCreateDto` => `TaskDto`) inside `McpService.AddTasksAsync`

- `Uid = Guid.NewGuid().ToString()` (new task — same as the Telegram `CreateTaskCommandProcessor`).
- Copy `Title`, `Date`, `Time`, `Type`, `IsProbable`.
- Leave `Order = 0`, `Completed = false`, `Deleted = false`, `Version = 0`. The service persists a
  brand-new entity (sets `Version = 1`) because the generated `Uid` does not exist yet.

### Validation (in `McpService.AddTasksAsync`)

- `justification` null/whitespace => `ArgumentException(nameof(justification))` (identical to
  `UpdateTasksOrderAsync`).
- `tasks` null / empty / containing a null element, or any `Title` null/whitespace =>
  `ArgumentException(nameof(tasks))`. Guard explicitly so unattended execution raises a controlled
  `ArgumentException` rather than a `NullReferenceException`.

### Notes on the DTO annotations

Only DTOs used as tool **inputs** (`TaskUpdateDto`, `TaskCreateDto`, and the nested `TaskTypeDto`)
actually surface in an MCP tool schema. `TaskDto` / `TasksUpdatedDto` are returned as serialized
JSON / broadcast payloads, so their `[Description]`s are documentation-only — added purely for the
consistency the user requested. `Models/AuthToken.cs` and `Models/AuthTokenBuildInfo.cs` are auth
models (not DTOs, not agent-facing) and are **excluded**.

## Validation

Fast per-task gate (from the repo's `.ralph/instructions.md` — run after EVERY task; a clean build
has **0 warnings**, warnings are errors here):

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

## Todos

### Task 1: Add the `TaskCreateDto` input DTO

**Files:**
- Create: `code/backend/DD.Shared.Details.Abstractions/Dto/TaskCreateDto.cs`

- [x] Create `TaskCreateDto` in namespace `DD.Shared.Details.Abstractions.Dto` with
      `using System.ComponentModel;`.
- [x] Add `[Description]`-annotated properties: `Title` (string, default `string.Empty`),
      `Date` (`DateTime?`), `Time` (`int?`, description states "minutes from midnight"),
      `Type` (`TaskTypeDto`), `IsProbable` (`bool`).
- [x] Verify the fast checks pass.

### Task 2: Annotate the existing DTOs with `[Description]`

**Files:**
- Modify: `code/backend/DD.Shared.Details.Abstractions/Dto/TaskDto.cs`
- Modify: `code/backend/DD.Shared.Details.Abstractions/Dto/TaskUpdateDto.cs`
- Modify: `code/backend/DD.Shared.Details.Abstractions/Dto/TasksUpdatedDto.cs`
- Modify: `code/backend/DD.Shared.Details.Abstractions/Dto/TaskTypeDto.cs`

- [x] Add `using System.ComponentModel;` and a `[Description]` on every `TaskDto` property
      (`Id`, `Date`, `Time` = minutes-from-midnight, `Title`, `Order`, `Completed`, `IsProbable`,
      `Deleted`, `Type`, `Version`, `Uid`).
- [x] Add `[Description]` on `TaskUpdateDto.Uid` and `TaskUpdateDto.Order` (add the `using`).
- [x] Add `[Description]` on `TasksUpdatedDto.Tasks`, `.UserId`, `.ClientId` (add the `using`).
- [x] Add `[Description]` on each `TaskTypeDto` enum member (`Simple`, `Additional`, `Routine`,
      `Weekly`) (add the `using`).
- [x] Verify the fast checks pass.

### Task 3: Add `AddTasksAsync` to `McpService` (with its log message)

**Files:**
- Modify: `code/backend/DD.McpClient.Domain/McpService.cs`
- Modify: `code/backend/DD.McpClient.Domain/Log.cs`

- [x] In `Log.cs`, add a `Log.AddTasks` partial method: `EventId = 5003`, `Level = Information`,
      message including the task count and the justification.
- [x] Add `Task<string> AddTasksAsync(ICollection<TaskCreateDto> tasks, string userId, string justification);`
      to the `IMcpService` interface.
- [x] Implement `AddTasksAsync` in `McpService`: throw `ArgumentException(nameof(justification))`
      when `justification` is null/whitespace; throw `ArgumentException(nameof(tasks))` when `tasks`
      is null, empty, contains a null element, or any `Title` is null/whitespace (guard explicitly
      so a bad payload raises a controlled `ArgumentException`, never a `NullReferenceException`).
- [x] Map each `TaskCreateDto` to a `TaskDto` with `Uid = Guid.NewGuid().ToString()`, copying
      `Title`/`Date`/`Time`/`Type`/`IsProbable`; call
      `taskServiceApp.SaveTasksAsync([.. mapped], userId, clientId: null)`; log via `Log.AddTasks`;
      serialize the result with the existing `JsonOptions` and return it.
- [x] Verify the fast checks pass.

### Task 4: Add `AddTasksTool` and register it

**Files:**
- Create: `code/backend/DD.Clients.Details/McpClient/Tools/AddTasksTool.cs`
- Modify: `code/backend/DD.Clients.Details/Setup.cs`

- [x] Create `AddTasksTool` mirroring `UpdateTasksOrderTool`: `internal sealed` class with
      `[McpServerToolType]`, and a static `Do` method with `[McpServerTool(Name = "AddTasks")]` +
      a `[Description]` summarizing the tool.
- [x] `Do` parameters: `IMcpService mcpService`, `IUserAuth userAuth`,
      `[Description(...)] ICollection<TaskCreateDto> tasks`, `[Description(...)] string justification`;
      resolve `var userId = userAuth.UserId();` and `return mcpService.AddTasksAsync(tasks, userId, justification);`.
- [x] In `DD.Clients.Details/Setup.cs`, register the tool by adding `.WithTools<AddTasksTool>()`
      after `.WithTools<UpdateTasksOrderTool>()`.
- [x] Verify the fast checks pass.

### Task 5: Add unit tests for `AddTasksAsync`

**Files:**
- Modify: `code/backend/DD.Tests.Unit/McpClient/McpServiceTests.cs`

- [x] Happy path: `AddTasksAsync` with one `TaskCreateDto` calls
      `SaveTasksAsync(It.Is<...>, "user-1", null)` with a mapped `TaskDto` (non-empty `Uid`;
      matching `Title`/`Date`/`Time`/`Type`/`IsProbable`) captured via a Moq callback, returns the
      serialized saved result, and logs the justification (assert via the logger mock like the
      existing `UpdateTasksOrderAsync` test).
- [x] `[Theory]` with `null` / `""` / `"   "` justification => `ArgumentException` with
      `ParamName == "justification"`; assert `SaveTasksAsync` is never called.
- [x] Invalid `tasks` collections each => `ArgumentException` with `ParamName == "tasks"` and
      `SaveTasksAsync` never called: a `null` collection, an empty collection, a collection with a
      `null` element, and a `TaskCreateDto` with a whitespace `Title`.
- [x] Verify the fast checks pass.

### Task 6: Final validation (full local end-to-end run — mandatory)

- [x] Build the whole backend solution with **0 warnings**:
      `dotnet build code/backend/DarkDeeds.sln -c Release`.
- [x] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`.
- [x] Run the full frontend gate: `cd code/frontend && npm run ci`.
- [x] Bring up the full local stack: `./infra/up.sh` (starts MongoDB **and** the Selenium Grid
      `test-e2e-chrome` on `:4444`); start the backend detached —
      `dotnet run --project code/backend/DD.App` on `:5000` (wait for
      `curl -fsS http://localhost:5000/healthcheck` => `Healthy`); start the frontend detached —
      `cd code/frontend && npm run dev` on `:3000` (wait for HTTP 200). Never pipe a server through
      `head`/`tail`; capture each PID.
- [x] Run the e2e suite through the Selenium Grid, headless, from the repo root:
      `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests`
      and confirm ALL run tests pass (expect `Failed: 0, Passed: 11, Skipped: 1` — the skip is the
      `[ProductionBuildFact]` offline test).
- [x] Fix any e2e failure and re-run until the whole e2e suite is green, then tear down the backend
      and frontend by PID (leave MongoDB and the Selenium Grid running).

## Notes

- **No frontend changes**: new tasks reach the web client via the existing SignalR `TaskUpdated`
  broadcast (triggered by `SaveTasksAsync` with `clientId: null`).
- **No date-range restriction** on creation — mirrors the Telegram bot and `SaveTasksAsync` (the
  14-day visible period is a display-only concern).
- **Coding standards**: the Release build treats warnings as errors (gerrkoff.CodingStandards), so
  new/modified files must be warning-clean (analyzer/StyleCop/XML-doc rules).
- Everything committed must be in English (code, comments, tests).
