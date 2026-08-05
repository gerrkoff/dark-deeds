---
applyTo: '**'
---
# Custom Project Rules

Project-specific rules for this repository, established through experience. Rule capture and routing follow the global Continuous Learning instructions; new durable rules that do not fit a skill are appended to the `# Learned Lessons` section below.

# Learned Lessons

Rules specific to this repository. **If a rule is here — follow it. No exceptions.**

## Code protection

**Never modify code marked with a `// important` comment without explicit developer approval.** When a developer asks that some code not be touched, mark it `// important` (note the reason if provided); never remove or ignore an existing `// important` marker.

## Language

**Everything in the repository MUST be in English** — code identifiers, comments, docstrings, commit messages, and test names/data (unless the business domain genuinely requires otherwise). Never introduce non-English text, and translate any you encounter while refactoring.

## Pattern consistency

**Before adding or changing code, preserve the existing patterns in the surrounding file/module** — naming conventions, ordering, error-handling style, validation flow, parsing semantics. Introduce a new pattern only when it is a clear improvement, and document the rationale.

## Terminal client rendering (Spectre.Console)

**Preserve the intentionally commented selected-task style alternatives in `TerminalStyles.ForTask`.** They are retained as a quick rollback option, not dead code.

**Initialize `Console.InputEncoding` and `Console.OutputEncoding` to UTF-8 before any terminal client I/O.** This preserves non-ASCII task text and keyboard input on Windows consoles that otherwise default to a legacy code page.

**For mixed styles on one terminal line, use a custom `Renderable` that emits separate `Segment`s.** The repository's Spectre.Console version applies only one style per `Text` and does not support a styled `Text.Append` overload.

**Wrap every full-screen alternate-buffer redraw in DEC synchronized output (`CSI ? 2026 h` / `CSI ? 2026 l`) and release it in a `finally` block.** Clearing before sequentially writing the header, content, and footer otherwise exposes partial frames as visible flicker during ordinary keyboard navigation.

**Keep terminal connection, snapshot loading, and save synchronization as independent UI states, rendered together at the top of the footer as a compact lowercase yellow status strip.** Use `offline` only from SignalR lifecycle events, `unsynced` only after a save has failed and until all queued local changes settle, and `stale` while snapshot reload is pending; render every active marker together and never replace them with verbose or transient retry messages.

**Store persistent terminal warnings as explicit application state, not `StatusMessage`.** `StatusMessage` is transient and ordinary input replaces it; a snapshot-retry warning must remain rendered across navigation and editing until a successful snapshot clears its dedicated state.

**When copying or re-emitting Spectre `Segment`s (e.g. clipping the terminal viewport), copy a `SegmentLine` with `list.AddRange(segmentLine)` or `foreach`, never the collection-expression spread `[.. segmentLine]`.** The spread injects `null` padding segments into the copy, which later throw `NullReferenceException` deep inside `Spectre.Console.Rendering.Segment.Merge` when the output is written. An empty `[]` is safe; only spreading a non-empty `SegmentLine` is affected.

**Render terminal day cards as one top-to-bottom stream with a blank line between cards, but no trailing blank line after the final card.** Up/Down navigates the immediately previous/next visible task across all sections; Left/Right navigates the previous/next non-empty rendered day and lands on its first task.

**For a collapsed dated Routine group, always render the summary when at least one nondeleted Routine task exists, and count only incomplete Routine tasks.** Therefore a day whose Routine tasks are all completed renders `+0 routine`; expanded Routine groups and No Date tasks do not render this summary.

**The terminal `r` shortcut globally toggles dated Routine visibility for every day and does not require a focused task.** Keep this as a boolean view mode so Routine tasks added or received on new dates while expanded are shown automatically; pressing `r` again collapses every dated Routine group.

**Preserve the terminal input reducer's `KeyChar` shortcut dispatch and normalize standard Russian-layout letters to their English-key equivalents immediately before that switch.** Keep the change minimal: use `ConsoleKey` only for arrows and service keys as before, and use the original localized `KeyChar` for editor and login text insertion.

**When scrolling upward to the first task of a clipped terminal card, use the preceding card header as the viewport anchor so labels such as `No Date` reappear.** Render Today in yellow, other dated headers in a subtle contrasting colour without edge markers, and indent Additional tasks twelve extra spaces.

## Terminal client storage

**Keep the terminal client portable by storing its default config and state under `data/` beside the executable.** Preserve `--state-root` as the explicit isolation override for tests and unattended self-tests.

## Terminal client releases

**Trigger terminal client releases only from `dd-terminal-v*` tags.** Create releases without generated notes so unrelated monorepo changes are not included automatically.

**Offer terminal release tagging only after `staging` has been pushed successfully.** An empty version skips the release; otherwise create the annotated `dd-terminal-v<version>` tag on the exact deployed staging commit and push that tag separately.

**Keep helper scripts used by `ci/deploy.sh` under `ci/deploy-helpers/`.**

**Publish the terminal client for `win-x64` as a `.zip` containing `dd-terminal.exe`; keep macOS and Linux packages as `.tar.gz`.** Cross-publish Windows from the release runner, but treat interactive Windows Terminal behavior as requiring separate manual verification.

**Use .NET SDK `8.0.100` across CI and Docker build surfaces, while `code/backend/global.json` uses `latestFeature` so local development can roll forward to an installed .NET 8 feature band.** Run terminal release and CI `dotnet` commands from `code/backend` so the nested `global.json` governs SDK resolution; `setup-dotnet` installing an SDK does not force selection when `global.json` is outside the command's working-directory ancestry.
