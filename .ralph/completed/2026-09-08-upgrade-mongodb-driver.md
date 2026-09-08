# Plan: Upgrade MongoDB Driver Dependencies

## Problem

The backend currently resolves MongoDB.Driver 3.1.0 in the shared data layer and MongoDB.Driver
3.0.0 through AspNetCore.Identity.Mongo. Those versions bring SharpCompress 0.30.1 and Snappier
1.0.0, which are reported by transitive vulnerability analysis.

Independently pinning newer compression libraries is unsafe because SharpCompress 0.48.x removed
APIs referenced by MongoDB.Driver 3.1.0. Upgrade the parent MongoDB.Driver dependency as a compatible
set while retaining MongoDB Server 4.4 and all existing application behavior.

## Approach

Set the central MongoDB.Driver version to 3.11.1. `DD.Shared.Details` already references the driver
directly; add a direct reference to `DD.ServiceAuth.Domain` so AspNetCore.Identity.Mongo cannot
independently resolve its minimum 3.0.0 driver. MongoDB.Driver 3.11.1 declares patched
SharpCompress 0.48.1 and Snappier 1.3.1 as its compatible runtime dependencies.

The repository does not configure Mongo wire compression in committed connection strings or
`MongoClientSettings`, so do not add artificial compressor behavior or permanent compatibility
infrastructure. Prove the upgrade with a forced dependency restore, hard transitive-vulnerability
inspection, the complete backend suite including real MongoDB integration tests, and the mandatory
local E2E gate.

## Validation

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

## Todos

### Task 1: Upgrade and verify MongoDB.Driver

**Files:**
- Modify: `code/backend/Directory.Packages.props`
- Modify: `code/backend/DD.ServiceAuth.Domain/DD.ServiceAuth.Domain.csproj`

- [x] Upgrade the central `MongoDB.Driver` version from 3.1.0 to 3.11.1 and leave the existing `DD.Shared.Details` package reference unchanged.
- [x] Add an ordinary direct `MongoDB.Driver` reference to `DD.ServiceAuth.Domain` so AspNetCore.Identity.Mongo resolves 3.11.1 instead of its minimum 3.0.0 dependency.
- [x] Run `dotnet restore code/backend/DarkDeeds.sln --force`, then verify `DD.Shared.Details` and `DD.ServiceAuth.Domain` resolve MongoDB.Driver 3.11.1, SharpCompress 0.48.1, and Snappier 1.3.1.
- [x] Run `dotnet list code/backend/DarkDeeds.sln package --vulnerable --include-transitive`; require that neither SharpCompress nor Snappier is reported, and do not use `NuGetAudit=false`, `NoWarn`, or warning demotion.
- [x] Verify the fast checks pass with 0 warnings and all existing integration tests remain green against MongoDB 4.4.

### Task 2: Final validation

**Files:**
- None (validation only)

- [x] From the repository root, run `dotnet build code/backend/DarkDeeds.sln -c Release`, then `dotnet test code/backend/DarkDeeds.sln -c Release`; fix every source compatibility issue, vulnerability finding, test failure, and warning.
- [x] Run `cd code/frontend && npm run ci`; fix every failure and warning, then return to the repository root.
- [x] Store owned server PIDs in `/tmp/dd-ralph-backend.pid` and `/tmp/dd-ralph-frontend.pid`; if port 5000 or 3000 is occupied, kill only a process proven to match the corresponding PID file, expected command, and port ownership, otherwise abort naming the occupied port.
- [x] Run `./infra/up.sh`, verify MongoDB and Selenium Grid on ports 27017/4444, start `dotnet run --project code/backend/DD.App` and `cd code/frontend && npm run dev` as detached processes with separate logs/PID files, and poll with deadlines until `curl -fsS http://localhost:5000/healthcheck` returns `Healthy` and `curl -fsS http://localhost:3000/` succeeds.
- [x] Run `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests` without piping; require `Failed: 0, Passed: 11, Skipped: 1`; after any fix stop owned servers, rerun affected gates, restart, wait, and rerun the complete E2E suite.
- [x] Kill only recorded backend/frontend PIDs, verify ports 5000/3000 are free, leave MongoDB/Grid running, and confirm no `dd-integration-tests-*` container remains after testhost exit.

## Notes

- Drafted against clean `master` HEAD `431a140c58767d38f95a1c46563840af8b3dce12`. Revalidate this plan and the resolved dependency graph if HEAD changes before Ralph executes it.
- MongoDB.Driver 3.11.1 declares SharpCompress 0.48.1 and Snappier 1.3.1; do not independently override those transitive packages.
- MongoDB Server and the Testcontainers image remain at version 4.4.
- No Mongo wire compression is configured in the repository, so this plan intentionally does not add compressor-specific runtime behavior.
- This plan must complete before `.ralph/2026-09-08-expand-backend-integration-tests.md` runs.
- One adversarial review pass was completed; its dependency-compatibility blocker was resolved by upgrading the parent driver rather than pinning an incompatible child package.
- All repository content must remain English, and code marked `// important` must not be modified.
