# Plan: Remove URI-string analyzer suppressions after adopting CodingStandards 0.1.4

## Problem

The backend carries inline `[SuppressMessage(... CA1054/CA1056 ...)]` attributes across 9 files.
They were added only to satisfy the URI-as-string analyzer family (CA1054 URI-like parameters,
CA1056 URI-like properties) enforced through the shared `gerrkoff.CodingStandards` NuGet package,
which sets `AnalysisLevel=latest-all` and — on Release/CI — `TreatWarningsAsErrors=true`.

Those rules (CA1054/CA1055/CA1056) have now been disabled globally in `gerrkoff.CodingStandards`
`0.1.4` (published to NuGet.org). Once dark-deeds adopts `0.1.4`, the suppressions are redundant
and must be deleted so the codebase stays warning-clean (warnings are build-breaking errors here).

## Approach

Adopt `0.1.4` and delete the redundant suppressions in **one atomic task**. The two changes cannot
be validated independently: while still on `0.1.3` the CA rules are active, so removing the
suppressions first fails the Release build with `CA1054/CA1056` errors; only the combined end state
(`0.1.4` + no suppressions) builds clean.

In each affected file the URI-string attribute is the only `[SuppressMessage]`, so
`using System.Diagnostics.CodeAnalysis;` becomes unused and must be removed too (otherwise `IDE0005`
unused-using breaks the Release build) — **with one exception**: `OAuthUrlService.cs` also uses
`[NotNullWhen(true)]` from that same namespace, so there the two `CA1054` attributes are removed but
the `using` is **kept** (removing it would cause `CS0246`). The unrelated `CA1000` suppression in
`code/backend/DD.ServiceAuth.Domain/OAuth/Models/OAuthResult.cs` (and its `using`) is left intact.

No production behavior changes — this is analyzer-config adoption plus attribute cleanup.

## Validation

Fast per-task gate (run after every task):

```
dotnet build code/backend/DarkDeeds.sln -c Release
dotnet test code/backend/DarkDeeds.sln -c Release
```

A clean build has **0 warnings** (warnings are treated as errors) — fix any before moving on.

## Todos

### Task 1: Adopt CodingStandards 0.1.4 and remove redundant URI-string suppressions

**Files:**
- Modify: `code/backend/Directory.Packages.props`
- Modify: `code/backend/DD.Clients.Details/TelegramClient/Dto/TelegramStartDto.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/OAuthSettings.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Models/AuthCodeModel.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Dto/OAuthAuthorizeRequestDto.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Dto/OAuthRedirectResponseDto.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Services/OAuthUrlService.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Services/AuthCodeService.cs`
- Modify: `code/backend/DD.ServiceAuth.Domain/OAuth/Services/OAuthFlowService.cs`
- Modify: `code/backend/DD.ServiceAuth.Details/Web/Controllers/OAuthController.cs`

- [x] In `code/backend/Directory.Packages.props` change `<PackageVersion Include="gerrkoff.CodingStandards" Version="0.1.3" />` to `Version="0.1.4"`, then run `dotnet restore code/backend/DarkDeeds.sln`
- [x] Remove the `CA1056` `[SuppressMessage]` attribute and the now-unused `using System.Diagnostics.CodeAnalysis;` from `TelegramStartDto.cs`, `OAuthSettings.cs`, and `AuthCodeModel.cs`
- [x] Remove both the `CA1054` and `CA1056` `[SuppressMessage]` attributes and the now-unused `using System.Diagnostics.CodeAnalysis;` from `OAuthAuthorizeRequestDto.cs` and `OAuthRedirectResponseDto.cs`
- [x] Remove the `CA1054` `[SuppressMessage]` attribute(s) and the now-unused `using System.Diagnostics.CodeAnalysis;` from `AuthCodeService.cs` (1 attribute), `OAuthFlowService.cs` (2 attributes), and `OAuthController.cs` (1 attribute)
- [x] In `OAuthUrlService.cs` remove ONLY the two `CA1054` `[SuppressMessage]` attributes and **keep** `using System.Diagnostics.CodeAnalysis;` (still required by `[NotNullWhen(true)]`)
- [x] Verify nothing remains — `grep -rn "CA105[456]" code/backend --include=*.cs | grep -v /obj/` prints nothing (no rule ids, no `SuppressMessage` referencing them) — and the fast checks pass (`dotnet build -c Release` with 0 warnings, unit tests green)

### Task 2: Final validation (full pass incl. mandatory local e2e)

**Files:**
- (no code changes; validation only — apply fixes to the Task 1 files if anything fails)

- [x] Build the whole backend solution with no warnings: `dotnet build code/backend/DarkDeeds.sln -c Release`
- [x] Run the full backend unit suite: `dotnet test code/backend/DarkDeeds.sln -c Release`
- [x] Run the full frontend gate: `cd code/frontend && npm run ci`
- [x] Bring up the full local stack: `./infra/up.sh` (starts MongoDB **and** the Selenium Grid `test-e2e-chrome` on `:4444`); start the backend detached — `dotnet run --project code/backend/DD.App` — and wait for `curl -fsS http://localhost:5000/healthcheck` => `Healthy`; start the frontend detached — `cd code/frontend && npm run dev` — and wait for `http://localhost:3000/` => HTTP 200 (never pipe a server to `head`/`tail`; record each PID)
- [x] Run the e2e suite through the Selenium Grid, headless (NOT the local ChromeDriver): `CONTAINER=true SELENIUM_GRID_URL=http://localhost:4444 URL=http://host.docker.internal:3000 BE_URL=http://localhost:5000 dotnet test code/tests/DarkDeeds.E2eTests` and confirm **ALL** run tests pass — expect `Failed: 0, Passed: 11, Skipped: 1` (the skip is the `[ProductionBuildFact]` offline test)
- [x] If anything fails, fix it and **re-run the entire final-validation sequence above (build => unit => frontend => e2e) until every check passes** — never conclude with a failing or un-run gate
- [x] Tear down the backend and frontend you started (stop each by PID); leave MongoDB and the Selenium Grid running

## Notes

- **Atomicity:** keep the bump and the suppression removals in one task — an intermediate state
  (suppressions removed while still on `0.1.3`, or `0.1.4` adopted with the suppressions still
  present) fails the warnings-as-errors Release build.
- **`OAuthUrlService.cs` keeps its `using`** — it uses `[NotNullWhen(true)]`; only the two `CA1054`
  attributes are removed. The other 8 files drop the now-unused `using System.Diagnostics.CodeAnalysis;`.
- **Leave `code/backend/DD.ServiceAuth.Domain/OAuth/Models/OAuthResult.cs` untouched** — its
  `CA1000` suppression + `using` are unrelated to this change.
- **English only** for everything committed (code, comments, commit messages); this change
  introduces no user-facing or non-English text.
- **Never modify code marked `// important`** without explicit approval (none of the target files
  carry that marker, but do not add or remove one).
- Prerequisite already satisfied: `gerrkoff.CodingStandards 0.1.4` is published and restorable from
  NuGet.org.
- Review: hardened via an adversarial `review-ralph-plan` pass (different model, gpt-5.3-codex) —
  the `OAuthUrlService.cs` `using` blocker and the final-gate iteration wording were fixed pre-save.
