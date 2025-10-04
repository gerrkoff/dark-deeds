---
mode: 'agent'
---
# Commit and Push Protocol

You have been triggered by a request to commit and push current changes. This protocol automates the workflow of creating a branch, committing all changes, and pushing to the remote repository.

## Your Task

<commit_push_workflow>
1. **Analyze Current Changes**
   - Check what files have been modified (staged and unstaged)
   - Review the nature of the changes to determine the appropriate branch type
   - Understand the context and purpose of the changes

2. **Determine Branch Type**
   - **fix/** - Bug fixes, error corrections, issue resolutions
   - **feat/** - New features, capabilities, or functionality additions
   - **chore/** - Maintenance tasks, dependency updates, tooling changes, refactoring
   - **docs/** - Documentation-only changes
   - **test/** - Test additions or modifications
   - **refactor/** - Code restructuring without changing functionality
   - **perf/** - Performance improvements
   - **style/** - Code style/formatting changes (not CSS styling)

3. **Generate Branch Name**
   - Format: `<type>/<descriptive-name>`
   - Use kebab-case (lowercase with hyphens)
   - Keep it concise but descriptive (3-6 words)
   - Include the main subject of changes
   - Examples:
     - `fix/version-script-semver-compatibility`
     - `feat/add-user-authentication`
     - `chore/update-dependencies`
     - `refactor/extract-payment-service`

4. **Create Descriptive Commit Message**
   - First line: Brief summary (50-72 chars)
   - Blank line
   - Body: Detailed explanation of changes
   - Use bullet points for multiple changes
   - Explain "why" not just "what"
   - Reference issue numbers if applicable

5. **Execute Git Operations**
   - Create and checkout new branch
   - Stage all changes
   - Commit with generated message
   - Push to remote with upstream tracking
</commit_push_workflow>

## Branch Type Decision Tree

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

## Commit Message Format

<commit_message_format>
```
<type>: Brief summary of changes (imperative mood)

- Detailed point about first major change
- Explanation of second change
- Why these changes were made
- Any important context or decisions

Examples or additional notes if needed.
```

**Guidelines**:
- Start summary with capital letter
- Use imperative mood: "Fix bug" not "Fixed bug"
- Don't end summary with period
- Keep summary under 72 characters
- Separate summary and body with blank line
- Wrap body at 72 characters
- Use bullet points for multiple changes
- Explain WHY, not just WHAT

**Examples**:

```
Fix version script to use SemVer 2.0 compatible format

- Replace all non-alphanumeric characters with dots instead of dashes
- Ensures .NET version-suffix compatibility with SemVer 2.0
- Fixes build errors for branches with dots in names (e.g., dependabot branches)
- Example: dependabot/npm_and_yarn/code/frontend/vite-5.4.20 -> dependabot.npm.and.yarn.code.frontend.vite.5.4.20
```

```
Add user authentication with JWT tokens

- Implement JWT token generation and validation
- Add login and registration endpoints
- Create authentication middleware for protected routes
- Store refresh tokens in secure HTTP-only cookies

This provides secure authentication mechanism for the application
and enables user session management across requests.
```
</commit_message_format>

## Execution Steps

<execution_steps>
**Step 1: Check Current State**
```bash
git status
```
- Verify there are changes to commit
- Identify modified files
- Note current branch

**Step 2: Create and Switch to New Branch**
```bash
git checkout -b <type>/<descriptive-name>
```

**Step 3: Stage All Changes**
```bash
git add -A
```
or if user specified specific files, stage only those

**Step 4: Commit Changes**
```bash
git commit -m "<commit message>"
```

**Step 5: Push to Remote**
```bash
git push -u origin <branch-name>
```

**Step 6: Provide Summary**
- Confirm branch created and pushed
- Show the branch name
- Display the commit message
- Provide the Pull Request creation URL if available
</execution_steps>

## User Interaction

<user_interaction>
**Before Executing**:
If the changes are complex or ambiguous:
1. Briefly describe your analysis of the changes
2. Propose the branch type and name
3. Show the commit message you'll use
4. Wait for confirmation if uncertain

**After Executing**:
1. Confirm successful completion
2. Show the branch name
3. Mention the commit hash
4. Provide next steps (e.g., PR creation link)

**If User Provides Specific Instructions**:
- User may specify branch name: use it exactly
- User may specify commit message: use it exactly
- User may want to stage specific files only: respect that
- Always ask for clarification if instructions are ambiguous
</user_interaction>

## Special Cases

<special_cases>
**Multiple Unrelated Changes**:
If changes span multiple concerns, suggest:
- Creating separate branches for each concern
- Committing related changes together
- Splitting into multiple PRs

**Sensitive Changes**:
For changes to CI/CD, deployment, or critical infrastructure:
- Use extra care in commit message
- Mention potential impact
- Suggest testing before merge

**Work in Progress**:
If changes are incomplete:
- Offer to add "WIP:" prefix to branch/commit
- Suggest draft PR creation
- Note that this is unfinished work

**Already on a Feature Branch**:
If already on a non-main branch:
- Ask if user wants to create a new branch
- Or commit and push to current branch
- Default: commit and push to current branch if it follows naming convention
</special_cases>

## Error Handling

<error_handling>
**If git operations fail**:
- Show the error message
- Explain what went wrong
- Suggest corrective actions
- Ask user how to proceed

**If no changes detected**:
- Inform user there are no changes to commit
- Check if they expected changes
- Suggest checking git status manually

**If push fails (e.g., conflicts)**:
- Explain the conflict situation
- Suggest pulling and resolving conflicts
- Offer to help with merge resolution

**If branch name already exists**:
- Inform user of conflict
- Suggest alternative name with timestamp or suffix
- Ask for user preference
</error_handling>

## Post-Execution Checklist

After successful execution:
- ✅ New branch created with appropriate type prefix
- ✅ All changes committed with descriptive message
- ✅ Branch pushed to remote with upstream tracking
- ✅ User informed of success with PR creation link
- ✅ Current branch confirmed

Remember: The goal is to make the commit-and-push workflow smooth, consistent, and maintainable while following best practices for branch naming and commit messages.
