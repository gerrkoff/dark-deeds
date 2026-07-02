# Ralph plan instructions — dark-deeds

Repo-scoped rules for **authoring** Ralph plans in this repository. The `add-ralph-plan` skill reads
this file and MUST fold every rule below into the plan it generates (Ralph's runner does not read
this file — the rules only take effect if they are materialized as concrete tasks/checkboxes).
These rules **augment and strengthen** the skill defaults; on conflict, the rules here win.

## Per-task validation (fast gate, after every task)

Keep the per-task `## Validation` cheap so Ralph gets quick feedback:

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

For tasks that touch `code/frontend/**`, also run the frontend gate:

```
cd code/frontend && npm run ci
```

A clean build has **0 warnings** (warnings are treated as errors here) — fix them before moving on.

## Final validation MUST include a local end-to-end run (mandatory)

Unit tests here reference only the `*.Domain` projects and never exercise the real ASP.NET
request/auth pipeline, so whole-app regressions (routing, auth, JWT/claims, DI, dependency bumps)
pass `dotnet build` + `dotnet test` and still ship broken. The **only** gate that catches them is the
Selenium e2e suite running against a live local stack. Therefore the plan's **final validation task**
MUST do all of the following and **iterate until every check passes** — never conclude with a failing
or un-run e2e gate (a per-test `Skipped` for the `[ProductionBuildFact]` offline test is expected and fine):

- [ ] Build the whole backend solution with no warnings: `dotnet build code/backend/DarkDeeds.sln -c Release`
- [ ] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`
- [ ] Run the full frontend gate: `cd code/frontend && npm run ci`
- [ ] Bring up the full local stack (see the local-run workflow in `.github/copilot-instructions.md`
      and the `run-e2e-tests` skill in `.github/skills/`): `./infra/up.sh` (starts **both** MongoDB
      **and** the Selenium Grid `test-e2e-chrome` on `:4444` — both are defined in
      `infra/docker-compose.yml`), the backend `dotnet run --project code/backend/DD.App` on
      `:5000` (wait for `curl -fsS http://localhost:5000/healthcheck` => `Healthy`), and the frontend
      `cd code/frontend && npm run dev` on `:3000` (wait for HTTP 200) — start the servers as
      detached background processes, never as a command piped to `head`/`tail`
- [ ] Run the e2e suite **through the Selenium Grid, headless** (NOT the local ChromeDriver — that
      depends on the machine's Chrome version and hangs/leaks on a mismatch). From the repo root:
      `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests`
      and confirm **ALL** run tests pass (expect `Failed: 0, Passed: 11, Skipped: 1` — the skip is the
      `[ProductionBuildFact]` offline test). `URL` uses `host.docker.internal` because the browser runs
      inside the Grid container; `BE_URL`/`SELENIUM_GRID_URL` use `localhost` because `dotnet test` runs
      on the host. Never pipe the run through `head`/`tail`.
- [ ] Fix any e2e failure and re-run until the whole e2e suite is green, then tear down the backend
      and frontend you started (stop each by PID; leave MongoDB and the Selenium Grid running)

## Notes

- Everything committed must be in English (code, comments, tests, commit messages).
- Never modify code marked `// important` without explicit approval.
