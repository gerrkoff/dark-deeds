# Plan: Remove MCP prompts and add an evidence parameter to the write tool

## Problem

The Dark Deeds MCP server ships two large hard-coded prompt files (`basic`,
`weekly-order`) that are no longer wanted. They add maintenance weight and are being
retired. Separately, the server's only write tool (`UpdateTasksOrder`) operates on the
honor system: it trusts the agent to reorder a user's tasks with no stated rationale.

Following the "Agents Lack Honor — design for evidence" pattern, the write tool must
require a `justification` argument so the agent is forced to articulate WHY the new
ordering is correct, and the backend records that rationale in its logs.

## Approach

Operate on the CURRENT architecture (standalone stdio `code/mcp/DarkDeeds.Mcp` plus the
backend REST `McpController`), so this change is independently shippable and does not
depend on the later OAuth/monolith migration.

- Delete the `Prompts/` folder and drop the `WithPromptsFromAssembly()` registration.
- Thread a required `justification` string from the `UpdateTasksOrder` MCP tool, through
  the REST endpoint, into `McpService`, where it is logged via the existing
  source-generated `Log` logger. Enforce non-empty rationale in `McpService`
  (`DD.McpClient.Domain`) — this is the durable enforcement point that survives the later
  monolith migration, which keeps `McpService`. No new storage is introduced; structured
  logging matches the existing pattern (`Log.UpdateTasksOrder`).

Ordering: change the MCP project first (remove prompts, then have the tool send the new
argument), then update the backend to accept, log, and require it. The tool talks to the
backend over an HTTP URL string, so there is no compile-time coupling between the two
sides and the build stays green after every task.

## Validation

After each task, run these commands to verify correctness:

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
dotnet build code/mcp/DarkDeeds.Mcp/DarkDeeds.Mcp.csproj -c Release
```

(All three pass on the current baseline: backend 0 warnings, 159 tests pass, MCP 0
warnings.)

## Todos

### Task 1: Remove MCP prompt logic

**Files:**
- Delete: `code/mcp/DarkDeeds.Mcp/Prompts/BasicPrompt.cs`
- Delete: `code/mcp/DarkDeeds.Mcp/Prompts/WeeklyOrderPrompt.cs`
- Modify: `code/mcp/DarkDeeds.Mcp/Program.cs`

- [x] Delete the entire `code/mcp/DarkDeeds.Mcp/Prompts/` directory (both prompt files)
- [x] Remove the `.WithPromptsFromAssembly()` call from the MCP server builder chain in `Program.cs`
- [x] Run `rg -n "WithPromptsFromAssembly|Prompts" code/mcp/DarkDeeds.Mcp` and confirm it returns no results
- [x] Verify `dotnet build code/mcp/DarkDeeds.Mcp/DarkDeeds.Mcp.csproj -c Release` succeeds with no warnings

### Task 2: Require and send a justification from the MCP write tool

**Files:**
- Modify: `code/mcp/DarkDeeds.Mcp/Tools/UpdateTasksOrderTool.cs`

- [ ] Add a required `string justification` parameter to `UpdateTasksOrderTool.Do`, decorated with `[Description(...)]` instructing the agent to explain why this reordering is correct (so the MCP framework presents it as a required argument)
- [ ] Append `&justification=` to the request URL using `Uri.EscapeDataString(justification)` so free-text is safely URL-encoded
- [ ] Verify `dotnet build code/mcp/DarkDeeds.Mcp/DarkDeeds.Mcp.csproj -c Release` succeeds with no warnings

### Task 3: Accept, log, and enforce the justification in the backend

**Files:**
- Modify: `code/backend/DD.McpClient.Domain/McpService.cs`
- Modify: `code/backend/DD.McpClient.Domain/Log.cs`
- Modify: `code/backend/DD.Clients.Details/McpClient/McpController.cs`

- [ ] Add a `string justification` parameter to `IMcpService.UpdateTasksOrderAsync` and its `McpService` implementation
- [ ] In `McpService.UpdateTasksOrderAsync`, throw an `ArgumentException` when `justification` is null or whitespace, so an empty rationale is never accepted (the `McpController` is not an `[ApiController]`, so do not rely on `[Required]` model validation)
- [ ] Update the source-generated `Log.UpdateTasksOrder` (`LoggerMessage`, EventId 5002) to include the `justification` text in the logged message, and pass it at the call site
- [ ] Add a `[Required] string justification` query parameter to `McpController.UpdateTasksOrder` and forward it into `mcpService.UpdateTasksOrderAsync`
- [ ] Verify `dotnet build code/backend/DarkDeeds.sln -c Release` succeeds with no warnings

### Task 4: Cover the changed write path with a unit test

**Files:**
- Create: `code/backend/DD.Tests.Unit/McpClient/McpServiceTests.cs`

- [ ] Add an xUnit test class that uses Moq to mock `ITaskServiceApp` and `ILogger<McpService>` (follow the AAA + `MethodName_Scenario_ExpectedResult` conventions used elsewhere in `DD.Tests.Unit`)
- [ ] Test that `UpdateTasksOrderAsync` with a non-empty justification forwards the updates and userId to `ITaskServiceApp.UpdateTasksAsync` and returns the serialized result
- [ ] Test that `UpdateTasksOrderAsync` throws `ArgumentException` when justification is null/empty/whitespace
- [ ] Test that `LoadTasksByDateAsync` forwards from/till/userId and serializes the result
- [ ] Verify `dotnet test code/backend/DarkDeeds.sln -c Release` passes

### Task 5: Final validation

- [ ] Run `dotnet build code/backend/DarkDeeds.sln -c Release` and confirm 0 warnings
- [ ] Run `dotnet test code/backend/DarkDeeds.sln -c Release` and confirm all tests pass
- [ ] Run `dotnet build code/mcp/DarkDeeds.Mcp/DarkDeeds.Mcp.csproj -c Release` and confirm 0 warnings
- [ ] Run `rg -n "WithPromptsFromAssembly|DarkDeeds\.Mcp\.Prompts" code/mcp` and confirm it returns no results

## Notes

- This plan is a prerequisite for the companion OAuth plan
  (`2026-07-01-mcp-oauth-authentication.md`): that plan recreates these tools in-process
  and assumes prompts are already gone and the `justification` evidence parameter already
  exists on the write path.
- `LoadTasks` is a read-only tool and intentionally does NOT get a justification parameter;
  evidence is only required on the write tool.
- The justification is recorded via structured logging (Serilog -> Loki in the deployed
  app), not persisted to a database — this matches the existing logging pattern and is the
  agreed scope for "the server records it".
- The MCP tool's required parameter is the real anti-confabulation control (the agent must
  supply it); the backend `ArgumentException` guard is defense-in-depth and the enforcement
  point that survives the later monolith migration.
