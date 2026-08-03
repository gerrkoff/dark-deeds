# Dark Deeds Terminal Client (`dd-terminal`)

`dd-terminal` is a keyboard-first terminal client for Dark Deeds. It renders the Overview workflow
(No Date, Overdue, the current two weeks, and future days) in an alternate-screen TUI, stays usable
offline, persists every accepted edit before it reaches the server, replays unsaved edits after a
restart, receives real-time updates over SignalR, and resolves optimistic-concurrency conflicts with
the same backend-wins semantics as the web client.

## Installation

Prebuilt, self-contained, single-file binaries are published for `osx-arm64`, `osx-x64`,
`linux-x64`, `linux-arm64`, and `win-x64`. Each release ships one archive and one checksum per
platform:

- macOS/Linux: `dd-terminal-<rid>.tar.gz` and `dd-terminal-<rid>.tar.gz.sha256`
- Windows: `dd-terminal-win-x64.zip` and `dd-terminal-win-x64.zip.sha256`

Download the archive for your platform, verify it, extract the binary, and run it:

```bash
# Verify the checksum (run from the directory that holds both files).
shasum -a 256 -c dd-terminal-osx-arm64.tar.gz.sha256   # macOS
sha256sum -c dd-terminal-linux-x64.tar.gz.sha256       # Linux

# Extract and run.
tar -xzf dd-terminal-osx-arm64.tar.gz
./dd-terminal --version
./dd-terminal --help
./dd-terminal --profile production
```

On Windows, compare the expected hash in the `.sha256` file with:

```powershell
Get-FileHash .\dd-terminal-win-x64.zip -Algorithm SHA256
Expand-Archive .\dd-terminal-win-x64.zip
.\dd-terminal-win-x64\dd-terminal.exe --version
```

The archive contains a single self-contained executable named `dd-terminal` or `dd-terminal.exe`;
no .NET runtime needs to be installed. On macOS, a downloaded binary may be quarantined by
Gatekeeper; clear the attribute with `xattr -d com.apple.quarantine dd-terminal` if it refuses to
launch.

### Build from source

The five binaries are produced by a script that any contributor can run locally (macOS or Linux;
requires the .NET 8 SDK):

```bash
ci/workflows/publish-terminal-client.sh <output-directory>
```

The script performs an untrimmed, self-contained, single-file `dotnet publish` for every runtime
identifier and writes deterministic `.tar.gz` or `.zip` archives plus `.sha256` checksums into the
output directory. The same script is invoked by the `Terminal Client Release` GitHub workflow
(`.github/workflows/terminal-client-release.yml`), which runs on `workflow_dispatch` and on
`dd-terminal-v*` tags.

After `ci/deploy.sh` successfully pushes `staging`, it prompts for an optional terminal version.
Entering a semantic version such as `1.2.0` creates and pushes the annotated tag
`dd-terminal-v1.2.0` on the exact deployed staging commit, which starts the release workflow.
Leaving the version empty skips the terminal release.

## Profiles

A profile is a named backend target. Three profiles are built in:

| Profile      | Base URL                        |
| ------------ | ------------------------------- |
| `production` | `https://dark-deeds.com/`       |
| `test`       | `https://test.dark-deeds.com/`  |
| `local`      | `http://localhost:5000/`        |

Select one with `--profile <name>` (default: `production`). Every profile isolates its own token,
cached tasks, outbox, settings, and logs, so signing in to `test` never touches `production` data.

Base URLs must use HTTPS, except for loopback hosts (`localhost`, `127.0.0.1`, `::1`), which may use
HTTP so the `local` profile can talk to a development backend.

### Custom profiles

The three built-in names are reserved. To add your own target, create a `profile.json` in a new
profile directory under the data root (see [Local state](#local-state-reset-and-migration)):

```text
<data-root>/profiles/<your-name>/profile.json
```

```json
{
  "BaseUrl": "https://your-instance.example.com/"
}
```

Then run `dd-terminal --profile <your-name>`. Profile names must be a single safe path segment (no
slashes, no `..`).

## Login and tokens

- On first use, or whenever no usable token is stored, the client opens a masked login prompt inside
  the TUI. Only the returned JWT is persisted; your password is never written to disk or logs.
- The token is stored separately from application state in `token.jwt`. On Unix it is created with
  permissions `0600` (owner read/write only); on Windows it inherits the data directory's ACL.
- While signed in, the client silently renews the token before it expires (when less than one day of
  lifetime remains).
- On a `401`, the client stops network activity, keeps your persisted outbox, and returns to the
  login prompt. Signing in again as the **same** user restores and replays any unsaved edits.

## Keyboard shortcuts

Normal mode:

| Keys                        | Action                                                                        |
| --------------------------- | ----------------------------------------------------------------------------- |
| Up / Down, `k` / `j`         | Move to the previous / next task in the rendered stream                        |
| Left / Right, `h` / `l`      | Move to the first task of the previous / next rendered day                     |
| `a`                         | Add a task using the focused task's date; with no focus, add a No Date task   |
| `A`                         | Add a No Date task                                                             |
| `e`                         | Edit the focused task                                                          |
| Space                       | Complete / uncomplete the focused task                                         |
| `d`                         | Enter delete confirmation                                                      |
| Shift+Up / Shift+Down, `K` `J` | Reorder relative to the visible neighbors                                  |
| Shift+Left / Shift+Right, `H` `L` | Move one calendar day (not available for No Date until a date is chosen) |
| `m`                         | Open the editor to move to an explicit date or to No Date                      |
| `r`                         | Toggle Routine visibility for the focused dated day                           |
| `c`                         | Toggle completed-task visibility (local only)                                 |
| `Ctrl+R`                    | Force a hub reconnect and full snapshot reload                                |
| `?`                         | Toggle help                                                                    |
| `q`                         | Quit                                                                           |

Modes:

- **Editor / login** treat printable keys as text and reserve only cursor/editing keys, Enter
  (commit), and Escape (cancel). The editor shows live parser feedback as you type.
- **Delete confirmation** accepts `y`, `n`, or Escape.
- **Help** closes with `?` or Escape.
- **Resize-required** keeps synchronizing and accepts only `?` and `q`.

Task text uses the same notation as the web client (for example, a leading `1730` sets 17:30, and a
date range expands into one task per day).

## Local state, reset, and migration

By default, all persistent data is portable and stored in a `data` directory beside the
`dd-terminal` executable. The executable directory must therefore be writable by the current user.

Per profile:

- `<data-root>/profiles/<profile>/profile.json` – custom profile definition (built-in profiles are
  not written to disk).
- `<data-root>/profiles/<profile>/state.json` – schema-versioned application state: data owner,
  cached tasks, durable outbox, and local completed-visibility toggle.
- `<data-root>/profiles/<profile>/token.jwt` – stored JWT (`0600`).
- `<data-root>/profiles/<profile>/logs/terminal.log` – bounded diagnostic log.

Pass `--state-root <dir>` to relocate all config and state under separate `config` and `state`
subdirectories of that directory (used by tests and the self-test to isolate a throwaway location).

**Migration.** `state.json` carries a schema version. On startup the client migrates older documents
forward through an explicit pipeline that never drops the outbox, so unsaved edits survive an upgrade.
A state file written by a newer version, or a malformed/unsupported one, is preserved untouched and
surfaced as a blocking, actionable error rather than being overwritten.

**Reset.** To start a profile from scratch, quit the client and delete that profile's data directory
(`<data-root>/profiles/<profile>/`). Deleting `token.jwt` alone forces a fresh login; deleting the
whole directory also clears cached tasks and any unsaved outbox edits.

## Conflict resolution

Synchronization is backend-wins, matching the web client:

- Each task carries a `version`. The client applies incoming tasks that have no local pending edit
  and ignores incoming versions that are the same as or older than a pending local edit.
- When the server reports a **newer** version for a task you have edited locally, the client drops the
  pending/in-flight local copy, applies the server task, persists the reduced outbox, and shows a
  conflict notice identifying the affected task.
- The client applies its own REST save response immediately (the server excludes the origin client
  from hub notifications), so it never waits for a hub echo to update versions.
- On a transport failure, in-flight edits are requeued, the durable outbox is left intact, and the
  save is retried after five seconds.

## Using a local backend

The `local` profile targets `http://localhost:5000/`, the port exposed by the all-in-one `DD.App`
host. From the repository root:

```bash
./infra/up.sh                                   # start MongoDB
dotnet run --project code/backend/DD.App        # backend on http://localhost:5000
dd-terminal --profile local                     # or: dotnet run --project code/backend/DD.TerminalClient -- --profile local
```

Loopback HTTP is permitted, so no TLS setup is needed for local development.

## Self-test

`--self-test` runs an unattended, real-backend contract check. It never enters the alternate screen,
so it is safe in CI and over non-interactive SSH.

Required environment variables:

- `DD_TERMINAL_USERNAME`
- `DD_TERMINAL_PASSWORD`

```bash
STATE_ROOT=$(mktemp -d)
DD_TERMINAL_USERNAME="$USERNAME" DD_TERMINAL_PASSWORD="$PASSWORD" \
  dd-terminal --profile local --self-test --state-root "$STATE_ROOT"
```

The self-test signs in, opens a writer and an observer hub connection with distinct client IDs, loads
the snapshot, then creates, updates/completes, and soft-deletes a uniquely named task, asserting REST
version assignment and the observer's real-time updates at each step. It cleans up the task it
created, uses a throwaway state root by default, exits non-zero with secret-safe diagnostics on any
failed assertion (usage errors, including missing credentials, exit with code `2`), and never prints
credentials or the JWT.

## Troubleshooting

- **"requires an interactive terminal".** Interactive mode refuses redirected/piped input or output.
  Run the client directly in a TTY, or use the non-interactive `--help`, `--version`, or
  `--self-test`.
- **Resize-required screen.** The Overview needs at least **120x30**. Below that, the client shows a
  resize prompt while continuing to synchronize in the background; only `?` and `q` are accepted.
  Enlarge the window to return to normal mode.
- **Offline / cached start.** If the backend is unreachable at startup, the client renders cached
  tasks immediately and retries in the background without blocking the UI.
- **Stale or stuck data.** Press `Ctrl+R` to force a hub reconnect and a full snapshot reload.
- **Unknown profile.** `dd-terminal: unknown profile '<name>'` means the name is neither built in nor
  present as a `profile.json`. Check the spelling and the data root path above.
- **Diagnostics.** Consult `<data-root>/profiles/<profile>/logs/terminal.log`. Logs never contain
  passwords, tokens, or authorization headers.

## Human-only terminal verification checklist

> This checklist is intentionally **manual**. It is **not** part of the automated build, tests, or any
> unattended completion gate, because it requires a real interactive terminal, a real remote host, and
> human eyes on the rendered output.

Verify by hand, ideally in [Ghostty](https://ghostty.org/):

- [ ] **Local Ghostty rendering.** Launch `dd-terminal` in Ghostty and confirm colors, borders, task
      styles (selected, today, completed, probable, Additional/Routine/Weekly, timed), and the
      header/footer render correctly.
- [ ] **Windows Terminal rendering.** Launch `dd-terminal.exe` in Windows Terminal and confirm input,
      colors, alternate-screen switching, resize handling, and screen restoration work correctly.
- [ ] **SSH PTY allocation.** Run the client over SSH with a PTY (`ssh -t <host> dd-terminal`) and
      confirm keyboard input and the alternate screen work.
- [ ] **Remote `xterm-ghostty` terminfo.** When connecting from Ghostty to a remote host, ensure the
      `xterm-ghostty` terminfo entry is installed there (or `TERM` is set to a known value) so styling
      is not garbled.
- [ ] **Resize.** Resize the window across the 120x30 threshold and confirm the resize-required screen
      appears/clears and that layout, viewport clipping, and continuation indicators recompute
      cleanly.
- [ ] **Alternate-screen restoration.** Quit with `q` and interrupt with `Ctrl+C`, and confirm the
      terminal cursor and the pre-launch screen contents are restored in both cases.
