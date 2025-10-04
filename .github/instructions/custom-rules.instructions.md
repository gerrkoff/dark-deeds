---
applyTo: '**'
---
# Custom Project Rules

This file contains project-specific rules that have been established through experience and learning. These rules supplement the general development guidelines.

<custom_rules>

### Important Code Protection
Code blocks marked with `// important` comments are critical and must not be modified without explicit permission.

When it applies:
- Any code with `// important` comment
- Code explicitly marked as protected by developers

What to do:
1. When you want to change such code, first ask the developer for approval
2. If a developer requests that certain code should not be touched, mark it with `// important`
3. Document the reason for protection if provided

What to avoid:
- Modifying protected code without explicit permission
- Removing or ignoring `// important` markers

### Language Consistency
Everything in the repository MUST be in English.

When it applies:
- All code files
- All documentation
- All comments (inline and block)
- Commit messages
- Test names and test data

What to do:
- Write all code identifiers (variables, functions, classes) in English
- Write all comments in English
- Use English for documentation strings
- Write commit messages in English
- Use English for test names and test data strings (unless business domain requires otherwise)

What to avoid:
- Introducing any non-English text in source code
- Leaving existing non-English text during refactoring
- Using non-English variable or function names

### Local Pattern Consistency
Before adding or changing code, preserve existing patterns in the surrounding file/module.

When it applies:
- Adding new code to existing files
- Refactoring existing code
- Implementing new features in established modules

What to do:
1. Scan the surrounding file/module for existing patterns
2. Note: naming conventions, ordering of assignments, error handling style, validation flow, parsing semantics
3. Preserve the existing structure and ordering
4. Only introduce a new pattern when there's a clear improvement
5. Document the rationale when introducing a new pattern

What to avoid:
- Inventing new variations of existing patterns without justification
- Mixing multiple coding styles within one file
- Breaking established conventions without documenting why

</custom_rules>
