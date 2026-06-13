---
name: push-pr
description: Push the current branch and create a pull request, without committing. Use when changes are already committed and you only need to push the existing commits to the remote and provide a pre-filled pull request link.
---
# Push and Pull Request Protocol

This skill automates the workflow of pushing the current branch and creating a pull request. Unlike the `commit-push-pr` skill, it does **not** create any commit — it works with the changes that have **already been committed** on the current branch.

## Your Task

<push_pr_workflow>
1. **Analyze Current State**
   - Check the current branch (it should not be `main`/`master`)
   - Identify the commits on this branch that are not yet on the base branch
   - Warn about any uncommitted changes (they will NOT be included, since this skill does not commit)
   - Understand the context and purpose of the existing commits

2. **Determine Branch (only if needed)**
   - If already on a feature branch: use it as-is
   - If on `main`/`master` with local commits: create a feature branch at the current `HEAD` (no commit is made) so a PR can be opened
   - Branch naming (when creating one) follows the conventional `<type>/<descriptive-name>` format described below

3. **Push to Remote**
   - Push the current branch to the remote with upstream tracking

4. **Generate Pull Request Link**
   - Derive the PR title and body from the existing commit(s) on the branch
   - Provide the GitHub Pull Request creation URL with pre-filled title and body
</push_pr_workflow>

## Branch Type Decision Tree

Use this when a branch must be created (you are on `main`/`master`) or to determine the `[<type>]` prefix for the PR title.

<branch_type_rules>
**Use `fix/` when**:
- Fixing a bug or error
- Correcting incorrect behavior
- Resolving a specific issue
- Patching a problem

**Use `feat/` when**:
- Adding new functionality
- Implementing a new feature
- Introducing new capabilities
- Expanding application features

**Use `chore/` when**:
- Updating dependencies
- Modifying build scripts or CI/CD
- Changing tooling configuration
- Refactoring code structure
- Updating documentation infrastructure
- General maintenance tasks

**Use `docs/` when**:
- ONLY documentation changes
- No code modifications
- README, guides, or API docs updates

**Use `test/` when**:
- Adding new tests
- Modifying existing tests
- Test infrastructure changes

**Use `refactor/` when**:
- Restructuring code without changing behavior
- Improving code quality or readability
- Extracting methods or classes
- No functional changes

**Use `perf/` when**:
- Optimizing performance
- Improving speed or efficiency
- Reducing resource usage

**Use `style/` when**:
- Code formatting changes
- Fixing linting issues
- No functional changes
</branch_type_rules>

## Deriving the Pull Request from Existing Commits

<pr_derivation>
**Determine the type**:
- If the current branch follows the `<type>/...` convention, use that type
- Otherwise, infer the type from the existing commit message(s) using the Branch Type Decision Tree

**Title format**: `[<type>] <Summary without type prefix>`
- For a single commit: use the commit summary
- For multiple commits: write a concise summary that captures the overall change
- Example: `[feat] Add auto-scroll for drag and drop on touch devices`

**Body format**:
- For a single commit: include the commit body (bullet points if present)
- For multiple commits: list the commits as bullet points, or summarize the key changes
- Explain "why" not just "what" when context is available
</pr_derivation>

## Execution Steps

<execution_steps>
**Step 1: Check Current State**
```bash
git status
git branch --show-current
```
- Confirm there are committed changes to push
- Note the current branch
- Identify uncommitted changes (warn the user — they will not be included)

**Step 2: Inspect Commits to Be Included**
```bash
git log origin/main..HEAD --oneline
```
- Review the commits that will be part of the PR
- Use these to derive the PR title and body
- (Replace `origin/main` with the actual base branch if different)

**Step 3: Create a Branch (only if on `main`/`master`)**
```bash
git checkout -b <type>/<descriptive-name>
```
- Skip this step if already on a feature branch

**Step 4: Push to Remote**
```bash
git push -u origin <branch-name>
```

**Step 5: Provide Summary**
- Confirm the branch was pushed
- Show the branch name
- List the commits included in the PR
- Provide the Pull Request creation URL
</execution_steps>

## User Interaction

<user_interaction>
**Before Executing**:
If the situation is complex or ambiguous (e.g., on `main`, many commits, uncommitted changes present):
1. Briefly describe your analysis of the current state and the commits to be pushed
2. Note any uncommitted changes that will be excluded
3. Propose the branch (if one needs to be created) and the PR title/body
4. Wait for confirmation if uncertain

**After Executing**:
1. Confirm successful completion
2. Show the branch name
3. List the commits included in the PR
4. **Provide GitHub Pull Request creation link with pre-filled title and body**:
   - Format: `https://github.com/gerrkoff/dark-deeds/compare/<branch-name>?expand=1&title=<pr-title>&body=<pr-body>`
   - **Title format**: `[<type>] <Summary without type prefix>`
     - Example: `[feat] Add auto-scroll for drag and drop on touch devices`
     - The type should match the branch type (feat, fix, chore, etc.)
   - **Body format**: Include the relevant commit body or a summary of the included commits
     - URL-encode the title and body properly
     - Use `%0A` for newlines in the body
     - Use `%2D` for dashes in bullet points or just `-`
   - This allows the user to immediately create a PR with pre-filled information

**If User Provides Specific Instructions**:
- User may specify the branch name (when one needs to be created): use it exactly
- User may specify the PR title/body: use it exactly
- Always ask for clarification if instructions are ambiguous
</user_interaction>

## Special Cases

<special_cases>
**Uncommitted Changes Present**:
This skill does NOT commit. If there are staged or unstaged changes:
- Warn the user that these changes will NOT be included in the push/PR
- Suggest using the `commit-push-pr` skill instead if they want those changes committed
- Proceed only with the already-committed changes

**Nothing to Push**:
If there are no commits ahead of the base branch:
- Inform the user there is nothing to push
- Suggest checking `git log` and `git status` manually

**Already on `main`/`master`**:
If local commits exist on the default branch:
- Create a feature branch at the current `HEAD` so a PR can be opened (no commit is made)
- Default the branch type and name from the existing commits

**Branch Already Pushed**:
If the branch already exists on the remote:
- Push the new commits (fast-forward)
- Still provide the PR creation link in case a PR has not been opened yet
</special_cases>

## Error Handling

<error_handling>
**If git operations fail**:
- Show the error message
- Explain what went wrong
- Suggest corrective actions
- Ask the user how to proceed

**If no commits to push**:
- Inform the user there are no committed changes to push
- Check if they expected commits (they may need `commit-push-pr` instead)
- Suggest checking `git log` manually

**If push fails (e.g., non-fast-forward)**:
- Explain the situation
- Suggest pulling and rebasing/merging before pushing again
- Offer to help resolve the conflict

**If branch name already exists locally (when creating one)**:
- Inform the user of the conflict
- Suggest an alternative name with a suffix
- Ask for the user's preference
</error_handling>

## Post-Execution Checklist

After successful execution:
- ✅ Current branch pushed to remote with upstream tracking
- ✅ No new commit was created (existing commits used as-is)
- ✅ Uncommitted changes (if any) reported to the user
- ✅ User informed of success with PR creation link
- ✅ Commits included in the PR listed

Remember: The goal is to push already-committed work and open a pull request smoothly, without creating any new commit.
