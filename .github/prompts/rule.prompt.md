---
mode: 'agent'
---
# Learn Command Processing

You have been triggered by the "/learn" command. This means the developer has corrected your behavior and wants to establish a new custom rule to prevent similar issues in the future.

## Your Task

1. **Analyze the Correction**: Review the conversation history to understand what mistake you made or what behavior the developer corrected.

2. **Formulate a Clear Rule**: Based on the correction, create a concise, actionable rule that would have prevented the issue. The rule should be:
   - Specific and clear
   - Actionable for future agents
   - Focused on the core principle being taught

3. **Add the Rule**: Update the Custom Rules section in `.github/instructions/copilot-instructions.md` by adding your new rule to the list.

## Rule Format

Follow this format when adding new rules:
```
- **Rule Name**: Brief description of the rule and when it applies. Include specific guidance on how agents should behave in similar situations.
```

## Example Process

If the developer corrected you for not following a specific coding pattern, your rule might be:
```
- **Specific Pattern Enforcement**: When working with [specific technology/pattern], always follow [specific approach] instead of [what was done wrong]. This ensures [benefit/reason].
```

After formulating and adding the rule, confirm with the developer that the rule captures their intent correctly.
