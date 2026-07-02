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
or skipped e2e run:

- [ ] Build the whole backend solution with no warnings: `dotnet build code/backend/DarkDeeds.sln -c Release`
- [ ] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`
- [ ] Run the full frontend gate: `cd code/frontend && npm run ci`
- [ ] Bring up the full local stack (see the local-run workflow in `.github/copilot-instructions.md`):
      MongoDB via `./infra/up.sh`, the backend `dotnet run --project code/backend/DD.App` on
      `:5000` (wait for `curl -fsS http://localhost:5000/healthcheck` => `Healthy`), and the frontend
      `cd code/frontend && npm run dev` on `:3000` (wait for HTTP 200) — start the servers as
      detached background processes, never as a command piped to `head`/`tail`
- [ ] Run the e2e suite against that local stack and confirm **ALL** tests pass:
      `dotnet test code/tests/DarkDeeds.E2eTests` (defaults to `URL=http://localhost:3000` /
      `BE_URL=http://localhost:5000`; uses a local ChromeDriver, so Chrome must be installed)
- [ ] Fix any e2e failure and re-run until the whole e2e suite is green, then tear down the backend
      and frontend you started (stop each by PID; leave MongoDB running)

## Notes

- Everything committed must be in English (code, comments, tests, commit messages).
- Never modify code marked `// important` without explicit approval.
