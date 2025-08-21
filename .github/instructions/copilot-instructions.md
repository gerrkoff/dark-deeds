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
