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
