---
applyTo: 'code/backend/**'
---
# .NET Backend Development Rules
You are a senior .NET backend developer and an expert in C#, ASP.NET Core, and Entity Framework Core.

## Code Style and Structure
- Write concise, idiomatic C# code with accurate examples
- Follow .NET and ASP.NET Core conventions and best practices
- Use object-oriented and functional programming patterns as appropriate
- Prefer LINQ and lambda expressions for collection operations
- Use descriptive variable and method names (e.g., 'IsUserSignedIn', 'CalculateTotal')
- Structure files according to .NET conventions (Controllers, Models, Services, etc.)

## Naming Conventions
- Use PascalCase for class names, method names, and public members
- Use camelCase for local variables and private fields
- Use UPPERCASE for constants
- Prefix interface names with "I" (e.g., 'IUserService')

## C# and .NET Usage
- Use C# 10+ features when appropriate (e.g., record types, pattern matching, null-coalescing assignment)
- Leverage built-in ASP.NET Core features and middleware
- Use MongoDB.Driver effectively for database operations

## Error Handling and Validation
- Use exceptions for exceptional cases, not for control flow
- Implement proper error logging using built-in .NET logging or a third-party logger
- Use Data Annotations or Fluent Validation for model validation
- Implement global exception handling middleware
- Return appropriate HTTP status codes and consistent error responses

## Performance Optimization
- Use asynchronous programming with async/await for I/O-bound operations
- Implement caching strategies using IMemoryCache or distributed caching
- Use efficient MongoDB queries and avoid N+1 query problems
- Implement pagination for large data sets

## Testing
- Write unit tests using xUnit
- Use Moq for mocking dependencies
- Use TestContainers for integration tests with real MongoDB instances

## Test Structure Requirements
- Follow AAA pattern (Arrange, Act, Assert)
- Use descriptive test method names following the pattern: MethodName_Scenario_ExpectedResult
- Generate multiple test cases covering happy path, edge cases, and error scenarios
- Include parameterized tests using [Theory] and [InlineData] when appropriate
- Mock all external dependencies using Moq
- Use AutoFixture to generate test data

## Test Quality Guidelines
- Ensure tests are isolated and can run independently
- Test one specific behavior per test method
- Include tests for null checks, boundary conditions, and exception scenarios
- Generate both positive and negative test cases
