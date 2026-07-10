---
name: verify-mcp-browser-sync
description: Verifies that Dark Deeds MCP write tools propagate to an already-connected browser through SignalR. Use when AddTasks or UpdateTasksOrder appears not to refresh the UI, or when checking MCP-to-browser real-time updates locally or on test.dark-deeds.com.
---

When using this skill, start your response with: ✅ Skill used: verify-mcp-browser-sync

# Verify MCP browser synchronization

Exercise the real MCP OAuth, tool, SignalR, frontend mapping, and Redux synchronization path with a disposable user. Do not treat a successful MCP response alone as proof that the browser updated.

## Procedure

1. **Prepare the app and browser.**
   - For local verification, follow the repository browser-testing lifecycle: ensure MongoDB is running, start `DD.App` on port 5000 and Vite on port 3000, and wait for both readiness checks.
   - Open the frontend in an isolated browser context.
   - For deployed verification, use `https://test.dark-deeds.com`; never create test data in production.

2. **Create and sign in a disposable user.**
   - Call `POST /api/test/CreateTestUser`.
   - Sign the browser in with the returned username and password.
   - Confirm the browser console reports that the task WebSocket is connected before invoking an MCP write tool.
   - Sign in through `POST /api/auth/Account/SignIn` as well and retain the application JWT for the OAuth consent request.

3. **Obtain an MCP OAuth access token.**
   - Register a public client through `POST /register` with a loopback redirect URI.
   - Generate a PKCE verifier and its S256 challenge.
   - Call `POST /authorize` with the application JWT and JSON containing `action: "allow"`, the client ID, redirect URI, challenge, and state.
   - Extract the authorization code from `redirectUrl`.
   - Exchange it through `POST /token` using `grant_type=authorization_code`, the code, redirect URI, client ID, and verifier.

4. **Initialize the Streamable HTTP MCP session.**
   - Send `initialize` to `/mcp` with both `application/json` and `text/event-stream` accepted.
   - Capture the `Mcp-Session-Id` response header.
   - Send `notifications/initialized` with that session ID before calling tools.

5. **Invoke and verify the write tool.**
   - Call `AddTasks` or `UpdateTasksOrder` through `tools/call`.
   - Confirm the MCP result contains the saved task with its new version.
   - In the connected browser, confirm a `Tasks online update` console entry contains that UID in `tasksToApply`.
   - Confirm the task appears or changes in the UI without a reload. Test both a no-date task and a task dated inside the visible period when investigating `AddTasks`.
   - Compare `AddTasks` and `UpdateTasksOrder` using the same browser session; both should traverse `TaskService => TaskServiceNotifier => TaskServiceSubscriber => TaskHub`.

6. **Clean up.**
   - Soft-delete temporary tasks through `POST /api/task/tasks` using the application JWT.
   - Close isolated browser pages and stop backend/frontend processes started for the check.
   - Leave shared MongoDB infrastructure running.
