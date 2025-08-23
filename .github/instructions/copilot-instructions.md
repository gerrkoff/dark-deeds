---
applyTo: '**'
---
# Development Rules

This project contains different technology stacks with specific development rules:

## Backend (.NET) Rules
For all files in `code/backend/**`, see [backend.instructions.md](backend.instructions.md)

## Frontend (React) Rules
For all files in `code/frontend/**`, see [frontend.instructions.md](frontend.instructions.md)

## General Project Guidelines
- Follow consistent coding standards across the project
- Write comprehensive tests for all new features
- Use meaningful commit messages
- Document any complex business logic

## Custom Rules
This section contains project-specific rules that have been established over time:

- **Important Code Protection**: Code blocks marked with "// important" comments are critical and should not be modified without explicit permission. When an agent wants to change such code, it must first ask the developer for approval. If during a conversation a developer requests that certain code should not be touched, mark it with "// important" comment.
- **Language Consistency**: Absolutely everything committed to the repository (code identifiers, inline/block comments, documentation strings, commit messages, test names, test data strings unless business domain demands otherwise) must be written in clear English. Do not introduce or leave any non-English text in source code, comments, tests, or commit messages.
- **Local Pattern Consistency**: Before adding or changing code, scan the surrounding file/module for existing patterns (naming, ordering of assignments, error handling style, validation flow, parsing semantics). If a similar construct already exists, preserve its structure and ordering instead of inventing a new variation. Only introduce a new pattern when there is a clear improvement and document the rationale.
- **Final Build Verification**: Before concluding any multi-step task (feature, refactor, fix, rule addition), perform ALL of the following checks. Treat any build error, test failure, or warning as critical; do not finish the task until everything passes cleanly (or an explicit developer exception is recorded). If warnings arise, iteratively fix them immediately.
	- Backend (.NET):
		1. `dotnet build DarkDeeds.sln -c Release`
		2. `dotnet test DarkDeeds.sln -c Release --nologo --verbosity minimal`
	- Frontend (React):
		1. `npm run build`
		2. `npm run fmt:check` (or `npm run fmt` if you intentionally formatted files as part of the task)
		3. `npm run test:run`
	- Success Criteria: All commands exit with code 0, no warnings in build output, and test suites are green.
	- Record in the task summary which commands were executed and their pass status.
