---
mode: 'agent'
---
# Custom Rule Learning Protocol

You have been triggered by a learning request. The developer has identified a behavior that needs correction and wants to establish a new custom rule to prevent similar issues in the future.

## Your Task

<learning_workflow>
1. **Analyze the Correction**
   - Review the conversation history carefully
   - Identify what mistake was made or what behavior was corrected
   - Understand the root cause and context of the issue
   - Note any patterns or edge cases involved

2. **Formulate a Clear Rule**
   - Create a concise, actionable rule that addresses the root cause
   - Ensure the rule is specific enough to prevent the issue
   - Make it general enough to apply to similar situations
   - Use clear, unambiguous language

3. **Add the Rule to Custom Rules**
   - Update the `<custom_rules>` section in `.github/instructions/custom-rules.instructions.md`
   - Place the rule in a logical position within the section
   - Follow the established format and style

4. **Verify and Confirm**
   - Ensure the rule doesn't contradict existing rules
   - Confirm with the developer that the rule captures their intent
   - Explain the reasoning behind the rule formulation
</learning_workflow>

## Rule Format Requirements

<rule_format>
Rules in the `<custom_rules>` section must follow this format:

```
### Rule Name
Brief description of the rule and its purpose.

When/Where it applies:
- Specific context or trigger conditions
- File patterns or code areas affected

What to do:
- Explicit actions to take
- Step-by-step guidance if needed
- Examples of correct behavior

What to avoid:
- Common mistakes to prevent
- Anti-patterns to watch for
```

**Key Principles**:
- Use `###` for the rule name (Markdown h3)
- Be specific about when the rule applies
- Provide actionable guidance, not just principles
- Include examples when helpful for clarity
- Keep language clear and unambiguous
</rule_format>

## Examples of Good Rules

<rule_examples>
**Example 1: Avoiding Over-Searching**
```
### Context Gathering Efficiency
When searching for implementation patterns, stop gathering context once you have sufficient information to proceed.

When it applies:
- During discovery phase before implementing features
- When looking for existing patterns in the codebase

What to do:
1. Start with targeted searches for specific patterns
2. If 2-3 files show the same pattern, adopt it
3. Stop searching once you can name the exact files to modify

What to avoid:
- Exhaustive searches across the entire codebase
- Searching for the same pattern multiple times
- Analysis paralysis from too much context
```

**Example 2: Technology-Specific Pattern**
```
### MongoDB Filter Usage
Always use strongly-typed filter builders for MongoDB queries instead of raw JSON filters.

When it applies:
- All MongoDB query operations in `code/backend/**`
- Filter construction in repository classes

What to do:
- Use `Builders<T>.Filter.Eq(x => x.Property, value)` syntax
- Leverage lambda expressions for type safety
- Combine filters with `&` operator

What to avoid:
- Raw BsonDocument filters: `new BsonDocument("field", "value")`
- String-based field names: `Filter.Eq("field", value)`
- Dynamic filter construction without type checking
```
</rule_examples>

## Process Guidelines

<learning_guidelines>
**Analysis Phase**:
- Read the entire conversation to understand full context
- Identify the specific moment where the correction occurred
- Note what you did, what you should have done, and why

**Formulation Phase**:
- Think deeply about the underlying principle being taught
- Consider edge cases and related scenarios where the rule applies
- Make the rule specific enough to be actionable but general enough to be reusable
- Ensure the rule aligns with existing workflow and quality standards

**Implementation Phase**:
- Add the rule to the `<custom_rules>` section in `.github/instructions/custom-rules.instructions.md`
- Maintain alphabetical or logical ordering if a pattern exists
- Use consistent formatting with existing rules
- Run verification builds to ensure the instructions file is valid

**Confirmation Phase**:
- Explain to the developer what rule you've formulated
- Describe your reasoning for the specific wording
- Ask if the rule captures their intent correctly
- Be open to refinement based on their feedback
</learning_guidelines>

## After Adding the Rule

Once you've added the rule:

1. **Summarize**: Briefly explain what rule was added and where
2. **Reasoning**: Describe why this specific formulation addresses the issue
3. **Confirm**: Ask the developer if the rule captures their intent
4. **Record**: Note that verification was performed (if builds were run)

Remember: The goal is not just to add a rule, but to ensure future agents will understand and follow the principle you're encoding.
