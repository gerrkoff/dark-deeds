# Plan: Add Backend Integration Tests

## Problem

Dark Deeds currently relies on mock-heavy unit tests for backend orchestration and on a Selenium
suite for whole-application confidence. Repository and specification mocks duplicate persistence
behavior and make backend changes painful, while adding most new scenarios to Selenium would slow
feedback and increase fragility.

Introduce an API integration layer that exercises the complete `DD.App` request, authentication,
dependency injection, migration, domain, repository, serialization, and MongoDB path. This first
delivery covers Auth and Tasks only, preserves every existing smoke/E2E test, and removes unit tests
only after stronger integration coverage exists.

## Approach

Create `code/backend/DD.Tests.Integration` with `Microsoft.AspNetCore.Mvc.Testing` 8.0.30 and
`Testcontainers.MongoDb` 4.15.0, reference `DD.App`, and add it to `DarkDeeds.sln`. Use the non-static
`DD.App.Startup` type as the `WebApplicationFactory<TEntryPoint>` marker and expose the existing
static `Program.CreateHostBuilder` method publicly so conventional host discovery builds the same
application without changing production behavior.

The repository uses xUnit v2, whose collection fixtures serialize all classes sharing the fixture.
To retain parallel test classes without migrating the existing test framework, implement a
process-wide `IntegrationEnvironment` backed by a static `Lazy<Task<IntegrationEnvironment>>`. It
starts one explicitly named `mongo:4.4` Testcontainer without replica-set configuration, creates one
random database, creates one shared `WebApplicationFactory<Startup>`, and forces host startup so
migrations complete before any test receives a client. Each test creates and disposes its own
`HttpClient`; authorization headers and mutable request state are never shared.

Parallel scenarios isolate state with unique policy-compliant usernames, task UIDs, and related
identifiers, and no test clears shared collections. Register idempotent cleanup callbacks for both
`AssemblyLoadContext.Default.Unloading` and `AppDomain.CurrentDomain.ProcessExit`. Cleanup must stop
the application factory first, delete the database through a separate Mongo client, and dispose the
container in a `finally` path, with bounded waits and Testcontainers resource reaping as the
abnormal-exit fallback. Later backend surfaces are out of scope and require separate Ralph plans.

## Validation

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

## Todos

### Task 1: Codify the backend test pyramid

**Files:**
- Modify: `.github/instructions/backend.instructions.md`

- [x] Add a `Backend Test Pyramid` section defining unit, API integration, and smoke/E2E responsibilities.
- [x] State that backend use cases involving HTTP, auth, persistence, migrations, repositories, serialization, or service orchestration default to API integration tests while every existing smoke test remains.
- [x] Scope the existing external-dependency mocking rule to unit tests; require real MongoDB in API integration tests while third-party services remain replaced at their boundary.
- [x] Add the selective migration rule: remove a unit test only with equivalent or stronger integration coverage, while pure parsers, date matrices, reducers, formatting, rendering, and client-local behavior remain unit-tested.
- [x] Verify the fast checks pass with 0 warnings.

### Task 2: Add the integration project and host seam

**Files:**
- Modify: `code/backend/Directory.Packages.props`
- Modify: `code/backend/DD.App/Program.cs`
- Modify: `code/backend/DarkDeeds.sln`
- Create: `code/backend/DD.Tests.Integration/DD.Tests.Integration.csproj`
- Create: `code/backend/DD.Tests.Integration/xunit.runner.json`

- [x] Add central package versions `Microsoft.AspNetCore.Mvc.Testing` 8.0.30 and `Testcontainers.MongoDb` 4.15.0, retaining the existing xUnit v2 packages.
- [x] Create the `net8.0` integration project with a project reference to `DD.App`, include `xunit.runner.json` in test output, and explicitly enable parallel test collections with the conservative algorithm.
- [x] Change only `Program.CreateHostBuilder` visibility from private to public; keep `Program` static and use `Startup`, not `Program`, as the factory marker type.
- [x] Add `DD.Tests.Integration` under the solution Tests folder and preserve all existing projects and configurations.
- [x] Verify the fast checks pass with 0 warnings.

### Task 3: Implement the shared parallel environment

**Files:**
- Create: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationEnvironment.cs`
- Create: `code/backend/DD.Tests.Integration/Infrastructure/DarkDeedsWebApplicationFactory.cs`
- Create: `code/backend/DD.Tests.Integration/Infrastructure/IntegrationTestBase.cs`
- Create: `code/backend/DD.Tests.Integration/Infrastructure/TestUserClient.cs`

- [x] Require `docker info` to succeed and run `docker image inspect mongo:4.4 >/dev/null 2>&1 || docker pull mongo:4.4` once before the first Testcontainers validation.
- [x] Implement static `Lazy<Task<IntegrationEnvironment>>` startup for one uniquely named `dd-integration-tests-*` Mongo container, one random database, and one shared factory without a replica-set connection option or shared xUnit collection.
- [x] Configure `WebApplicationFactory<Startup>` with environment `Testing`, the test Mongo connection, deterministic Auth/OAuth values, `Monitoring:MetricsEnabled=false`, `EnableTelegramIntegration=false`, and `EnableTestHandlers=false`.
- [x] Force factory startup and migration completion before publishing the environment; expose factory client creation and unique-data helpers without exposing mutable shared host configuration.
- [x] On partial initialization failure and process teardown, dispose the factory first, attempt bounded database deletion, and guarantee bounded container disposal in `finally`; guard duplicate callbacks and report cleanup failures without hiding the original failure.
- [x] Verify the fast checks pass with 0 warnings, then require `docker ps --filter name=dd-integration-tests --format '{{.Names}}'` to return no running container after the test process exits.

### Task 4: Cover startup and authentication

**Files:**
- Create: `code/backend/DD.Tests.Integration/StartupIntegrationTests.cs`
- Create: `code/backend/DD.Tests.Integration/AuthIntegrationTests.cs`

- [x] Prove a fresh random database starts `DD.App`, completes hosted migrations, and serves anonymous healthcheck and build-info requests without a separately running backend or MongoDB.
- [x] Cover anonymous current-user, successful SignUp, successful SignIn with a unique policy-compliant credential, authenticated current-user, and token renewal through the real Account endpoints.
- [x] Assert wrong credentials return HTTP 200 with `SignInResult.WrongUsernamePassword`, duplicate sign-up returns HTTP 200 with `SignUpResult.UsernameAlreadyExists`, and a protected Tasks request without a bearer token returns HTTP 401.
- [x] Keep authorization state in a test-owned `HttpClient` so parallel tests never share or overwrite bearer headers.
- [x] Verify the fast checks pass with 0 warnings, no external service call occurs, and no `dd-integration-tests-*` container remains afterward.

### Task 5: Cover the Tasks lifecycle

**Files:**
- Create: `code/backend/DD.Tests.Integration/TasksIntegrationTests.cs`

- [x] Create no-date and dated tasks through `POST /api/task/tasks` and load them through `GET /api/task/tasks?from=...`.
- [x] Assert title, date, time, type, probable flag, order, UID, and version fields from public responses.
- [x] Re-POST a task with its current version and changed title/order, then assert the POST response and later GET contain the changes with an incremented version.
- [x] Re-POST a task with `Deleted=true` and its current version, then assert the delete response and immediate GET contain the tombstone with `Deleted=true` and an incremented version.
- [x] For the deleted no-date task, issue another GET with `from` more than seven days after deletion and assert the expired tombstone is absent without modifying production filtering.
- [x] Verify the fast checks pass with 0 warnings and no `dd-integration-tests-*` container remains afterward.

### Task 6: Cover filters, isolation, conflicts, and parallelism

**Files:**
- Create: `code/backend/DD.Tests.Integration/TasksIsolationIntegrationTests.cs`

- [x] Through HTTP, prove current/future tasks and an overdue incomplete Simple task are returned, while an overdue completed dated task is excluded.
- [x] Create two authenticated users, prove neither can load the other's task, and prove a foreign re-POST returns HTTP 200 with an empty array while the owner's later GET remains unchanged.
- [x] Create and update a task, submit its stale pre-update version, assert the stale task is absent from the HTTP 200 POST response, and verify the later GET still returns the winning value/version.
- [x] Re-POST several tasks with their current versions and new `Order` values in one request, then verify every order and incremented version through a later GET.
- [x] Coordinate two independent user task flows concurrently with `Task.WhenAll`, then run `for i in 1 2 3; do dotnet test code/backend/DD.Tests.Integration/DD.Tests.Integration.csproj -c Release || exit 1; done` to expose identifier, auth, order, and cleanup leakage.
- [x] Verify the fast checks pass repeatedly with 0 warnings and no `dd-integration-tests-*` container remains afterward.

### Task 7: Remove only replaced TaskService units

**Files:**
- Delete: `code/backend/DD.Tests.Unit/ServiceTask/Services/TaskServiceTests/TaskServiceTest.LoadActualTasksAsync.cs`
- Modify: `code/backend/DD.Tests.Unit/ServiceTask/Services/TaskServiceTests/TaskServiceTest.SaveTasksAsync.cs`

- [x] Delete the repository/specification interaction-only `LoadActualTasksAsync_Positive` test only after the HTTP filter cases pass.
- [x] Remove `SaveTasksAsync` unit cases replaced by integration scenarios for create, update, foreign ownership, soft delete, and stale-version omission; retain unmatched edge cases.
- [x] Leave `TaskServiceTest.LoadTasksByDateAsync.cs`, `TaskServiceTest.UpdateTasksAsync.cs`, shared mock helpers, recurrence tests, Terminal Client tests, parsers, rendering, and unrelated units unchanged because this REST slice does not cover those paths.
- [x] Keep the remaining TaskService setup compiling without broad test or production refactoring.
- [x] Verify the fast checks pass with 0 warnings and no `dd-integration-tests-*` container remains afterward.

### Task 8: Run integration tests in backend CI

**Files:**
- Modify: `.github/workflows/ci.yml`

- [x] Add a `docker info` preflight to the existing backend job so Docker/Testcontainers failures are explicit.
- [x] Build `DarkDeeds.sln` in Release and run `dotnet test DarkDeeds.sln -c Release --no-build`; do not add a Mongo service container or job-level container.
- [x] Ensure the solution-wired integration project runs on the existing `ubuntu-latest` Docker daemon with no staging deployment or repository secret.
- [x] Preserve all other CI jobs, `.github/workflows/tests-integration.yml`, and Selenium sources unchanged.
- [x] Verify the workflow YAML and fast checks pass with 0 warnings and no `dd-integration-tests-*` container remains afterward.

### Task 9: Final validation

**Files:**
- None (validation only)

- [x] From the repository root, run `dotnet build code/backend/DarkDeeds.sln -c Release`, then `dotnet test code/backend/DarkDeeds.sln -c Release`; fix all failures and warnings.
- [x] Run `cd code/frontend && npm run ci`; fix all failures and warnings, then return to the repository root.
- [x] Require `! nc -z localhost 5000 && ! nc -z localhost 3000`, run `./infra/up.sh`, and verify MongoDB plus Selenium Grid are ready on ports 27017 and 4444 without killing unknown processes.
- [x] Start `dotnet run --project code/backend/DD.App` and `cd code/frontend && npm run dev` as detached background processes with separate log files and recorded PIDs; poll until the backend healthcheck returns `Healthy` and the frontend returns HTTP 200.
- [x] Run `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests` without piping output; require `Failed: 0, Passed: 11, Skipped: 1`, fix failures, and repeat affected gates until green.
- [x] Kill only the recorded backend/frontend PIDs, verify ports 5000/3000 are free, leave MongoDB/Grid running, and require `docker ps --filter name=dd-integration-tests --format '{{.Names}}'` to be empty.

## Notes

- Drafted against repository HEAD `0eaba2044381749eb2e2a59cbbac0547617412b1` with a clean worktree. Revalidate this plan and every referenced contract if HEAD changes before Ralph executes it.
- This plan intentionally stops after the Auth/Tasks vertical slice. Recurrences, Web BFF settings, SignalR, Telegram, MCP/OAuth, and Mobile require separate follow-up Ralph plans.
- `TaskService.UpdateTasksAsync` and `LoadTasksByDateAsync` are not exposed by the Tasks REST controller, so their unit tests remain until later protocol integration plans cover them.
- A second application factory and process-restart persistence test are intentionally excluded to preserve the agreed one-factory design.
- Two adversarial review passes were completed. The remaining implementation risk is the xUnit v2 process-lifetime fixture; the plan therefore requires explicit bounded process hooks, partial-startup cleanup, repeated parallel runs, and mechanical container-leak checks.
- TestServer does not replace Kestrel, browser, reverse-proxy, packaging, service-worker, or real WebSocket coverage; the existing E2E suite remains mandatory.
- All committed code, tests, comments, and documentation must be English. Do not modify code marked `// important` without explicit approval.
