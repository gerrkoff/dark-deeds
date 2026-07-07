# Plan: Fix the recurrence from/until date input mask

## Problem

The recurrence editor's "From" and "Until" date fields use a custom "type digits, auto-insert
slashes" mask so a user can type `08072026` and see `08/07/2026`. The logic lives as three plain
top-level functions in `code/frontend/src/recurrences/components/EditRecurrenceModal.tsx`
(`handleDateChange`, `getDateInitialValue`, `getDateFromInput`) and is buggy:

1. **Backspace deletes two characters** — every deletion path runs an extra `inputValue.slice(0, -1)`
   on top of the char the browser already removed (`08/07/2026` + backspace => `08/07/20`).
2. **Caret ignored / mid-string edits corrupt the value** — the string is always rebuilt from the
   left and the extra trim always drops the last char, regardless of caret position.
3. **Weak validation** — `new Date(y, m-1, d)` normalizes overflow, so `31/02`, `45/13`, and
   partial/empty inputs all report valid; because `Number("") === 0`, an incomplete "From" silently
   becomes epoch 0 (1970) via `getDateFromInput(...) ?? new Date(0)`.
4. **Initial value not zero-padded** — an existing recurrence shows `8/7/2026`; editing it then
   corrupts (digits `872026` => `87/20/26`).
5. **Locale mismatch (display vs input)** — the input mask is a fixed `dd/mm/yyyy`, but the summary
   prints dates via `new Date(...).toLocaleDateString()` (browser locale, e.g. `en-US` =>
   `mm/dd/yyyy`), so what you type displays reordered.

## Approach

Keep the custom masked text input, but rewrite the masking + validation correctly and extract it
into a tested singleton service (repo convention: no top-level functions; services under
`src/<feature>/services/`, unit-tested with vitest under `code/frontend/tests/`; no React Testing
Library — tests target pure service logic).

- Add a locale-independent, zero-padded `toShortDate(date): string` (`dd/mm/yyyy`) to the shared
  `DateService`; reuse it for the input's initial value AND the recurrence summary.
- Create `DateMaskService` (singleton, injected with `DateService`) owning:
  - `applyMask(raw, prev)` — digit-based masking; append a section slash only while typing, never
    while deleting (fixes double-delete and backspace-over-slash).
  - `isValidDate(value)` — strict calendar check (exactly 8 digits + round-trip that rejects
    overflow like `31/02`, `45/13`).
  - `toTimestamp(value)` — value => epoch ms, or `null` when invalid.
  - `fromTimestamp(dateNumber)` — epoch ms => padded `dd/mm/yyyy` (or `''` for nullish / `0`).
- Rewire `EditRecurrenceModal` to the service and delete the three top-level functions (keep the
  existing `fromRef`/`toRef` prev-value tracking; initialize the validity flags from the actual
  initial values so Save can never start enabled with a bad date).
- Point `RecurrenceService.printDate` at `dateService.toShortDate` so the summary matches input.

Masking algorithm (digit-based):

```
applyMask(raw, prev):
  isDeleting = raw.length < prev.length
  digits = raw.replace(/\D/g, '').slice(0, 8)
  value  = digits.slice(0, 2)
  if digits.length > 2: value += '/' + digits.slice(2, 4)
  if digits.length > 4: value += '/' + digits.slice(4, 8)
  if !isDeleting and (digits.length == 2 or digits.length == 4): value += '/'
  return value
```

Strict validation:

```
isValidDate(value):
  digits = value.replace(/\D/g, '')
  if digits.length != 8: return false
  d,m,y = the three '/'-split parts as numbers
  date = new Date(y, m-1, d)
  return date.getDate()==d && date.getMonth()==m-1 && date.getFullYear()==y
```

"Until" keeps its empty-allowed rule at the call site (`isValidDate(value) || value === ''`).

## Validation

Fast per-task gate — Ralph runs this after EVERY task. Per the repo's `.ralph/instructions.md` the
backend build+test is mandatory even for frontend-only work, plus the frontend gate for the
`code/frontend/**` changes this plan makes. A clean build is 0 warnings (warnings are errors):

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
cd code/frontend && npm run ci
```

## Todos

### Task 1: Add locale-independent toShortDate to DateService

**Files:**
- Modify: `code/frontend/src/common/services/DateService.ts`
- Modify: `code/frontend/tests/services/DateService.test.ts`

- [x] Add `toShortDate(date: Date): string` returning zero-padded `dd/mm/yyyy` (e.g. `08/07/2026`), locale-independent (build from `getDate()`, `getMonth()+1`, `getFullYear()`, padding day and month to two digits); do NOT change `toDateLabel`.
- [x] Add vitest cases in `DateService.test.ts`: single-digit day and month are zero-padded (`new Date(2026,6,8)` => `08/07/2026`); a two-digit day/month is preserved (`new Date(2026,10,21)` => `21/11/2026`); the year is the full four digits.
- [x] Verify the fast checks pass.

### Task 2: Create DateMaskService with masking + strict validation

**Files:**
- Create: `code/frontend/src/recurrences/services/DateMaskService.ts`
- Create: `code/frontend/tests/services/DateMaskService.test.ts`

- [x] Create `DateMaskService` (constructor injects `DateService`) with `applyMask(raw: string, prev: string): string`, `isValidDate(value: string): boolean`, `toTimestamp(value: string): number | null`, `fromTimestamp(dateNumber: number | null | undefined): string`, and export `const dateMaskService = new DateMaskService(dateService)`.
- [x] Implement `applyMask` per the digit-based algorithm (strip non-digits, cap at 8 digits, insert `/` after day and month, append a trailing `/` after 2 or 4 digits only when NOT deleting).
- [x] Implement `isValidDate` (exactly 8 digits + `new Date` round-trip equality) and `toTimestamp` (`isValidDate` ? epoch ms : `null`); implement `fromTimestamp` (nullish or `0` => `''`, else `this.dateService.toShortDate(new Date(dateNumber))`).
- [x] Add vitest masking cases: typing `08072026` => `08/07/2026`; auto-slash appears after 2 and after 4 digits while typing; backspace removes exactly one char (`08/07/2026` => `08/07/202`); backspacing over an auto-added slash (`08/` => `08`) does not re-add it or drop a digit; non-digits are stripped; input capped at 8 digits (`0807202699` => `08/07/2026`).
- [x] Add vitest validation cases: `isValidDate` accepts `08/07/2026` and `29/02/2024` (leap year); rejects `31/02/2026`, `45/13/2026`, `29/02/2023` (non-leap), `00/01/2026`, `01/00/2026`, `''`, and partial `08/07`; `toTimestamp('08/07/2026')` === `new Date(2026,6,8).valueOf()` and `toTimestamp` returns `null` for invalid/empty; `fromTimestamp(new Date(2026,6,8).valueOf())` => `08/07/2026`, `fromTimestamp(null)` => `''`, `fromTimestamp(0)` => `''`.
- [x] Verify the fast checks pass.

### Task 3: Wire DateMaskService into EditRecurrenceModal and delete the top-level functions

**Files:**
- Modify: `code/frontend/src/recurrences/components/EditRecurrenceModal.tsx`

- [x] Import `dateMaskService`; initialize `from` via `dateMaskService.fromTimestamp(recurrence?.startDate ?? new Date().valueOf())` and `to` via `dateMaskService.fromTimestamp(recurrence?.endDate)`.
- [x] Initialize the validity flags from the actual initial values (not a bare `true`): `isFromValid` via `useState(() => dateMaskService.isValidDate(from))` and `isToValid` via `useState(() => dateMaskService.isValidDate(to) || to === '')`, so Save can never start enabled with an invalid date.
- [x] Rewrite `handleFromChange` to `const value = dateMaskService.applyMask(e.target.value, fromRef.current)` then set `fromRef.current = value`, `setFrom(value)`, `setIsFromValid(dateMaskService.isValidDate(value))`; rewrite `handleToChange` the same way with `setIsToValid(dateMaskService.isValidDate(value) || value === '')`.
- [x] In the `editRecurrence` memo use `startDate: dateMaskService.toTimestamp(from) ?? new Date(0).valueOf()` and `endDate: dateMaskService.toTimestamp(to)` (the epoch fallback is an unreachable typed placeholder — Save is gated on `isValid`).
- [x] Delete the three now-unused top-level functions `handleDateChange`, `getDateInitialValue`, `getDateFromInput` from the file.
- [x] Verify the fast checks pass.

### Task 4: Normalize the recurrence summary date format

**Files:**
- Modify: `code/frontend/src/recurrences/services/RecurrenceService.ts`

- [ ] Change `printDate(date: number)` from `new Date(date).toLocaleDateString()` to `this.dateService.toShortDate(new Date(date))` so the summary's from/until dates render as fixed `dd/mm/yyyy` matching the input.
- [ ] Verify the fast checks pass.

### Task 5: Final validation (full pass + live e2e)

- [ ] Build the whole backend solution with no warnings: `dotnet build code/backend/DarkDeeds.sln -c Release`.
- [ ] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`.
- [ ] Run the full frontend gate: `cd code/frontend && npm run ci`.
- [ ] Bring up the full local stack: `./infra/up.sh` (starts MongoDB AND the Selenium Grid `test-e2e-chrome` on `:4444`, both defined in `infra/docker-compose.yml`); start the backend detached `dotnet run --project code/backend/DD.App` on `:5000` and wait for `curl -fsS http://localhost:5000/healthcheck` => `Healthy`; start the frontend detached `cd code/frontend && npm run dev` on `:3000` and wait for HTTP 200 (record each PID; never pipe a server to `head`/`tail`).
- [ ] Run the e2e suite through the Selenium Grid, headless (NOT the local ChromeDriver): `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests`; confirm ALL run tests pass (expect `Failed: 0, Passed: 11, Skipped: 1` — the skip is the `[ProductionBuildFact]` offline test).
- [ ] Fix any e2e failure and re-run until the whole e2e suite is green, then tear down the backend and frontend you started (stop each by PID; leave MongoDB and the Selenium Grid running).

## Notes

- **Caret-in-the-middle preservation is a non-goal.** The rewrite fixes the double-delete and
  end-of-field backspace; full `setSelectionRange` caret restoration for mid-string edits is out of
  scope (people typically clear and retype a date field).
- **Overview task labels stay `en-US`.** `DateService.toDateLabel` (used by the overview) is left
  unchanged; only the recurrence summary is normalized to `dd/mm/yyyy`.
- **`fromTimestamp` treats `0`/nullish as `''`.** This preserves the prior `getDateInitialValue`
  semantics (`!dateNumber => ''`). A persisted recurrence always has a real `startDate` (and a
  `null` or real `endDate`), so `0` only ever arises from the never-saved invalid-input state.
- **The `startDate: ... ?? new Date(0).valueOf()` fallback is unreachable for persistence.** The
  `Save changes` submit button is `disabled={!isSaveEnabled}` with `isSaveEnabled={isValid}`
  (`code/frontend/src/common/components/ModalContainer.tsx`), and a disabled submit control blocks
  both click and Enter submission, so an invalid/epoch date can never be persisted; the fallback is
  only a type-satisfying placeholder.
- **`DateMaskService` lives under `recurrences/services/`** (its only consumer); it can move to
  `common/services/` later if reused.
- **Per-task validation includes the backend build+test** even though this plan is frontend-only,
  because the repo's `.ralph/instructions.md` mandates it as the fast gate; the backend is untouched
  so it should stay green throughout.
