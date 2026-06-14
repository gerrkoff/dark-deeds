# Development Rules

## Core Workflow

You are an expert development agent. Follow this workflow for all non-trivial tasks:

<workflow>
1. **Plan**: Decompose the request, identify ambiguities, outline approach
2. **Gather Context**: Efficiently collect necessary information using targeted searches
3. **Execute**: Implement changes following established patterns and best practices
4. **Verify**: Test thoroughly before concluding
</workflow>

## Quality Standards

<self_reflection>
For complex or high-impact tasks (new features, architectural changes, multi-file refactors):
1. Before acting, create a mental rubric with 5-7 quality categories relevant to the task
2. Think deeply about what makes for a world-class solution in each category
3. Internally iterate on your solution until it scores ≥98/100 across all categories
4. Never show this rubric to the user—it's for your internal quality assurance only
5. Only proceed when confident the solution meets the highest standards
</self_reflection>

## Context Gathering Rules

<context_gathering>
**Goal**: Get enough context fast. Parallelize discovery and stop as soon as you can act.

**Method**:
- Start with semantic or targeted searches; read relevant files in parallel
- Avoid over-searching for context. If signals converge (~70%) on one area, stop gathering
- Never repeat searches for the same information

**Early Stop Criteria**:
- You can name exact files/symbols to change
- Search results converge on one clear area/pattern
- You have sufficient context to make an informed decision

**Depth**:
- Only trace symbols you'll modify or whose contracts you directly rely on
- Avoid transitive expansion unless absolutely necessary for correctness

**Escalation**:
- If signals conflict or scope is unclear, run ONE more focused search batch, then proceed
- Search again only if verification fails or new unknowns appear
- Prefer acting with reasonable assumptions over endless searching
</context_gathering>

## Execution Efficiency

<execution_efficiency>
When executing tasks, optimize for efficiency:
- **Parallel Tool Calls**: When multiple independent operations are needed, execute them in parallel
  - Multiple file reads that don't depend on each other
  - Independent validation checks
  - Separate search queries for different information
- **Sequential Only When Necessary**: Use sequential execution only when:
  - Tasks have dependencies (output of one feeds into another)
  - Operations share state or modify the same resources
  - Order matters for correctness
- **Batch Operations**: Group similar operations together when possible
</execution_efficiency>

## Persistence and Autonomy

<persistence>
You are an autonomous agent. For all tasks:
- Keep going until the request is COMPLETELY resolved before ending your turn
- Decompose queries into ALL required sub-tasks and confirm each is completed
- Only terminate when you are certain the problem is fully solved
- Never stop at uncertainty—research or deduce the most reasonable approach and continue
- Do not ask for confirmation on reasonable assumptions—document them, act on them, adjust if proven wrong
- When in doubt about minor decisions, choose the approach most consistent with existing patterns
</persistence>

## Progress Communication

<tool_preambles>
Balance efficiency with transparency:
- For multi-step tasks: Briefly outline your plan upfront (2-3 sentences)
- During execution: Provide concise progress updates at major milestones
- After completion: Summarize what was accomplished
- Keep text concise but code changes verbose and readable
- Use clear variable names, proper formatting, and comments where helpful for reviewers
</tool_preambles>

## Project Context

For an understanding of the project's purpose, domain concepts, and key features, see [project-overview.instructions.md](instructions/project-overview.instructions.md)

## Technology Stack Rules

This project contains different technology stacks with specific development rules:

### Backend (.NET) Rules
For all files in `code/backend/**`, see [backend.instructions.md](instructions/backend.instructions.md)

### Frontend (React) Rules
For all files in `code/frontend/**`, see [frontend.instructions.md](instructions/frontend.instructions.md)

## Project-Specific Rules

For custom project rules established through experience and learning, see [custom-rules.instructions.md](instructions/custom-rules.instructions.md)

## Local Development Environment

<local_development>
The application can be run locally for manual or browser-based verification. `DD.App` is an all-in-one host (monolith) that exposes both the REST API and the SignalR hub on port `5000` and only depends on MongoDB; RabbitMQ and Consul are replaced by in-process implementations.

**Prerequisites**: Docker, .NET 8 SDK, Node.js 22.

**Components** (three separate processes, all started from the repository root):

| Component | How it runs | Port | Readiness check |
| --- | --- | --- | --- |
| MongoDB | Docker container `dd-mongo-db-1` via the `infra` scripts | `27017` | `nc -z localhost 27017` |
| Backend (`DD.App`) | `dotnet run --project code/backend/DD.App` (uses the `DarkDeeds.Backend.App` launch profile (`ASPNETCORE_ENVIRONMENT=Development`), binds `0.0.0.0:5000`) | `5000` | `curl -fsS http://localhost:5000/healthcheck` → `Healthy` |
| Frontend (Vite) | `cd code/frontend && npm run dev` (`strictPort: 3000`) | `3000` | `curl -fsS http://localhost:3000/` → HTTP 200 |

**Important rules**:
- **MongoDB is managed through the `infra` scripts** (Docker Compose project `dd`). Start it with `./infra/up.sh` and stop it with `./infra/down.sh`. Do **not** run a second `docker compose ... up mongo-db` without `-p dd` — that creates a duplicate container fighting for port `27017`.
- **Never start a server as a sync command piped to `head`/`tail`** (e.g. `dotnet run ... | head`). That ties the server's lifetime to the pipe so it dies immediately, which is what causes the "start it again and again" loop. Start the backend and frontend as **detached background processes** (bash tool with `mode:"async", detach:true`), redirect output to a log file (e.g. `/tmp/dd-backend.log`, `/tmp/dd-frontend.log`), and **record the returned PID** so you can stop them later.
- The frontend dev build targets the backend at `http://<hostname>:5000` (see `code/frontend/src/common/api/BaseUrlProvider.ts`). When you open `http://localhost:3000`, the backend must be reachable at `http://localhost:5000`.
</local_development>

## Browser / Manual Testing Workflow

<browser_testing_workflow>
Whenever you need to exercise the running app (Chrome DevTools MCP, manual REST calls, data checks), follow this exact lifecycle. **Always set the app up yourself and tear it down afterwards** — do not assume servers are already running, and never leave them running when you finish.

**1. Ensure MongoDB is up.**
- Check: `docker ps --filter name=dd-mongo-db --filter status=running --format '{{.Names}}'` (or `nc -z localhost 27017`).
- If it is **not** running, start it via the infra scripts from the repository root: `./infra/up.sh` (idempotent — only starts what is missing; Docker Compose project `dd`).

**2. Start the backend and the frontend (two separate apps), detached.**
- Backend: `dotnet run --project code/backend/DD.App` — run detached, log to `/tmp/dd-backend.log`, capture the PID.
- Frontend: `cd code/frontend && npm run dev` — run detached, log to `/tmp/dd-frontend.log`, capture the PID.
- Wait until each is ready before testing:
  - Backend: poll `curl -fsS http://localhost:5000/healthcheck` until it returns `Healthy`.
  - Frontend: poll `curl -fsS http://localhost:3000/` until it returns HTTP 200.

**3. If a server fails to start because its port is already in use** (Kestrel "address already in use" on `5000`, or Vite `strictPort` error on `3000`):
- Do **not** hunt for and kill processes yourself.
- **Ask the user** (via the `ask_user` tool) to stop the app they already have running on that port.
- Only if the user confirms they have **nothing** running there, *then* investigate: identify the process (`lsof -i :5000` / `lsof -i :3000`, `ps`), report what you found, and resolve it (e.g. `kill <PID>` for a confirmed stale process) before retrying.

**4. Run your tests.**
- **Browser**: use Chrome DevTools MCP tools (navigate, click, fill, read console/network). App URL: `http://localhost:3000`. Test credentials: username `test`, password `test`.
- **Database**: use MongoDB MCP tools (aggregate, find, count) to verify data after backend changes.

**5. Tear down when finished.**
- Stop the **backend and frontend you started** with `kill <PID>` for each captured PID, then verify the ports are free (`nc -z localhost 5000` and `nc -z localhost 3000` should both fail).
- **Leave MongoDB running** (it is shared infra managed by Docker; stop it only with `./infra/down.sh`, and only if explicitly asked).
- Rationale: the user may want to run or test the app themselves, so never leave your backend/frontend processes occupying ports `5000`/`3000`.
</browser_testing_workflow>

## Final Verification Protocol

<verification>
Before concluding ANY multi-step task (feature, refactor, fix, rule addition):

**Core Verification Principles**:
1. Verify all requirements from the original request are addressed
2. Check for internal consistency and correctness
3. Ensure changes follow established patterns
4. Run appropriate build and test commands per technology stack
5. Fix any warnings immediately—do not conclude with warnings present

**Technology-Specific Verification**:
- For backend changes in `code/backend/**`: Follow the `<verification_checklist>` in [backend.instructions.md](instructions/backend.instructions.md)
- For frontend changes in `code/frontend/**`: Follow the `<verification_checklist>` in [frontend.instructions.md](instructions/frontend.instructions.md)

**Success Criteria**:
- All build commands exit with code 0
- No warnings in build output
- All test suites are green
- All checklist items from the relevant verification_checklist are satisfied
- Record which verification steps were completed in your completion summary

**If Verification Fails**:
- Diagnose the issue
- Fix it immediately
- Re-verify
- Repeat until all checks pass
- Never hand back to the user with failing tests or build errors
</verification>
