---
name: run-e2e-tests
description: Run the DarkDeeds Selenium e2e suite locally the reliable way — headless, against the Selenium Grid that ships in infra, with zero dependency on the locally-installed Chrome. Use whenever you need to run code/tests/DarkDeeds.E2eTests locally (e.g. a Ralph final-validation e2e gate, or verifying a whole-app change). Prevents the local ChromeDriver/Chrome version-mismatch hang that wastes minutes per test.
---

When using this skill, start your response with: ✅ Skill used: run-e2e-tests

# Run e2e tests (headless, via Selenium Grid)

## Why this skill exists

`code/tests/DarkDeeds.E2eTests` pins `Selenium.WebDriver.ChromeDriver` to one major
version. Its `BaseTest.CreateDriver()` has **two** modes:

- **local ChromeDriver** (default) — drives the Chrome installed on your machine. If your
  local Chrome major version ≠ the pinned driver, **every test spawns a chromedriver that
  can't create a session, hangs ~1-2 min, then leaks the process**. No session, no backend
  calls — the run looks stuck. This is the trap that burned a whole Ralph session.
- **Selenium Grid** (`CONTAINER=true`) — connects a `RemoteWebDriver` to the
  `selenium/standalone-chrome` container, which bundles a **matching Chrome + driver** and
  runs **headless**. The local Chrome version is irrelevant.

**Always run local e2e through the Grid.** Never fall back to the local-ChromeDriver path,
never download/patch a matching chromedriver, never "wait longer" for a hung run.

## The Grid already lives in infra

`infra/docker-compose.yml` (Docker Compose project `dd`) defines **both** services you need:

- `mongo-db` -> host `:27017`
- `test-e2e-chrome` -> `selenium/standalone-chrome`, host `:4444`, with
  `extra_hosts: host.docker.internal:host-gateway` so the browser inside the container can
  reach apps running on your host.

`./infra/up.sh` brings both up (idempotent). `./infra/down.sh` stops them.

## Recipe (copy-paste)

Run every command from the repo root.

**1. Bring up infra (Mongo + Grid):**
```bash
./infra/up.sh
# readiness: `nc -z localhost 27017` and `nc -z localhost 4444` both succeed
```

**2. Start the backend and frontend, detached** (they must be reachable from the Grid
container). Both already bind `0.0.0.0` (`DD.App` launch profile; Vite `server.host: true`).
```bash
# backend -> http://localhost:5000
nohup dotnet run --project code/backend/DD.App > /tmp/dd-backend.log 2>&1 &
# frontend -> http://localhost:3000
( cd code/frontend && nohup npm run dev > /tmp/dd-frontend.log 2>&1 & )
```
Wait until ready (record the PIDs so you can stop them after):
```bash
# backend: curl -fsS http://localhost:5000/healthcheck  => Healthy
# frontend: curl -fsS -o /dev/null -w '%{http_code}' http://localhost:3000/  => 200
```

**3. Run the suite against the Grid (headless):**
```bash
CONTAINER=true \
SELENIUM_GRID_URL=http://localhost:4444 \
URL=http://host.docker.internal:3000 \
BE_URL=http://localhost:5000 \
dotnet test code/tests/DarkDeeds.E2eTests
```
Expected: **`Passed! - Failed: 0, Passed: 11, Skipped: 1`** (~45 s). The 1 skip is
`OfflineTests` (`[ProductionBuildFact]`, needs `PROD_BUILD_TESTS=true` + a production PWA
build — leave it skipped for a dev split-origin run). Do NOT pipe the run through
`head`/`tail`; that hides progress and can kill it. Use `| tee /tmp/dd-e2e.log` if you want a log.

**4. Tear down what you started** (leave the shared infra running):
```bash
# kill the backend and frontend by the PIDs you recorded (find them via
# `lsof -nP -tiTCP:5000 -sTCP:LISTEN` / `:3000`), then confirm 5000 and 3000 are free.
# Leave Mongo (:27017) and the Grid (:4444) up — they are shared infra.
```

## The one thing that trips people up: which host in which variable

The `dotnet test` process runs on your **host**; the browser runs **inside the Grid
container**. `host.docker.internal` resolves **only inside the container**, not on the macOS
host. So the variables split by *who consumes them*:

| Var | Consumed by | Value | Why |
| --- | --- | --- | --- |
| `URL` | the **browser** (`driver.Navigate()`, `OpenNewTab`) | `http://host.docker.internal:3000` | container -> host frontend via host-gateway |
| `BE_URL` | host-side `BackendApi` (REST, create test user) | `http://localhost:5000` | host -> host backend; **never injected into the browser** |
| `SELENIUM_GRID_URL` | host-side `RemoteWebDriver` + `DriverNetworkExtensions` | `http://localhost:4444` | host -> Grid via published port |
| `CONTAINER` | `BaseTest` | `true` | selects Grid + headless mode |

The SPA computes its own backend as `http://<window.location.hostname>:5000`, so loading the
page from `host.docker.internal:3000` makes the browser call `host.docker.internal:5000`
automatically — that is why `BE_URL` stays `localhost` and is not a contradiction.

**Vite must allow the Grid's Host header.** Vite 5 blocks `host.docker.internal` with
`403 Blocked request` unless it is in `server.allowedHosts` (already added in
`code/frontend/vite.config.ts`; dev-server-only, no production effect). If a fresh checkout
of the frontend 403s from the Grid, re-add `allowedHosts: ['host.docker.internal']`.

## Alternative: fully containerized (how CI runs it)

`ci/workflows/test-e2e.sh <FE_URL> [BE_URL]` runs the **test project itself** in a container
on a throwaway network with its own Grid — no host apps, no local .NET needed, but you must
pass a host/LAN URL both the test container and the browser can reach (e.g.
`http://192.168.1.5:3000`), not `localhost`. Prefer the host-side Grid recipe above for
iterating locally; use this only to reproduce CI exactly.
