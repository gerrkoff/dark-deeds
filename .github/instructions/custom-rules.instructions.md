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

**When copying or re-emitting Spectre `Segment`s (e.g. clipping the terminal viewport), copy a `SegmentLine` with `list.AddRange(segmentLine)` or `foreach`, never the collection-expression spread `[.. segmentLine]`.** The spread injects `null` padding segments into the copy, which later throw `NullReferenceException` deep inside `Spectre.Console.Rendering.Segment.Merge` when the output is written. An empty `[]` is safe; only spreading a non-empty `SegmentLine` is affected.

**Render terminal day cards as one top-to-bottom stream with a blank line between cards, but no trailing blank line after the final card.** Up/Down navigates the immediately previous/next visible task across all sections; Left/Right navigates the previous/next non-empty rendered day and lands on its first task.

**When scrolling upward to the first task of a clipped terminal card, use the preceding card header as the viewport anchor so labels such as `No Date` reappear.** Render Today in yellow, other dated headers in a subtle contrasting colour without edge markers, and indent Additional tasks four extra spaces.

## Terminal client storage

**Keep the terminal client portable by storing its default config and state under `data/` beside the executable.** Preserve `--state-root` as the explicit isolation override for tests and unattended self-tests.

## Terminal client releases

**Trigger terminal client releases only from `dd-terminal-v*` tags.** Create releases without generated notes so unrelated monorepo changes are not included automatically.

**Offer terminal release tagging only after `staging` has been pushed successfully.** An empty version skips the release; otherwise create the annotated `dd-terminal-v<version>` tag on the exact deployed staging commit and push that tag separately.

**Keep helper scripts used by `ci/deploy.sh` under `ci/deploy-helpers/`.**

**Publish the terminal client for `win-x64` as a `.zip` containing `dd-terminal.exe`; keep macOS and Linux packages as `.tar.gz`.** Cross-publish Windows from the release runner, but treat interactive Windows Terminal behavior as requiring separate manual verification.
