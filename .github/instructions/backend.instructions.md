---
applyTo: 'code/backend/**'
---
# .NET Backend Development Rules
You are a senior .NET backend developer and an expert in C#, ASP.NET Core, and Entity Framework Core.

## Code Style and Structure

<code_standards>
- Write concise, idiomatic C# code with accurate examples
- Follow .NET and ASP.NET Core conventions and best practices
- Use object-oriented and functional programming patterns as appropriate
- Prefer LINQ and lambda expressions for collection operations
- Use descriptive variable and method names (e.g., 'IsUserSignedIn', 'CalculateTotal')
- Structure files according to .NET conventions (Controllers, Models, Services, etc.)
</code_standards>

## Naming Conventions

<naming>
- Use PascalCase for class names, method names, and public members
- Use camelCase for local variables and private fields
- Use UPPERCASE for constants
- Prefix interface names with "I" (e.g., 'IUserService')
- Choose names that clearly express intent and avoid abbreviations
</naming>

## C# and .NET Usage

<dotnet_patterns>
- Use C# 10+ features when appropriate (e.g., record types, pattern matching, null-coalescing assignment)
- Leverage built-in ASP.NET Core features and middleware
- Use MongoDB.Driver effectively for database operations
- Prefer dependency injection over static dependencies
- Use configuration options pattern for settings
</dotnet_patterns>

## Error Handling and Validation

<error_handling>
- Use exceptions for exceptional cases, not for control flow
- Implement proper error logging using built-in .NET logging or a third-party logger
- Use Data Annotations or Fluent Validation for model validation
- Implement global exception handling middleware
- Return appropriate HTTP status codes:
  - 200 OK for successful GET/PUT
  - 201 Created for successful POST
  - 204 No Content for successful DELETE
  - 400 Bad Request for validation errors
  - 401 Unauthorized for authentication failures
  - 403 Forbidden for authorization failures
  - 404 Not Found for missing resources
  - 500 Internal Server Error for unexpected errors
- Provide consistent error response structure
</error_handling>

## Performance Optimization

<performance>
- Use asynchronous programming with async/await for I/O-bound operations
- Implement caching strategies using IMemoryCache or distributed caching when appropriate
- Use efficient MongoDB queries and avoid N+1 query problems
- Implement pagination for large data sets
- Use projection to select only needed fields from MongoDB
- Consider using indexes for frequently queried fields
</performance>

## Testing Requirements

<testing_standards>
### Test Structure
- Follow AAA pattern (Arrange, Act, Assert)
- Use descriptive test method names following the pattern: `MethodName_Scenario_ExpectedResult`
- Generate multiple test cases covering:
  - Happy path (successful execution)
  - Edge cases (boundary conditions, empty inputs)
  - Error scenarios (null inputs, invalid data, exceptions)
- Include parameterized tests using [Theory] and [InlineData] when appropriate

### Test Implementation
- Use xUnit as the testing framework
- Use Moq for mocking dependencies
- Use TestContainers for integration tests with real MongoDB instances
- Use AutoFixture to generate test data and reduce boilerplate
- Mock all external dependencies (databases, external services, file systems)

### Test Quality
- Ensure tests are isolated and can run independently in any order
- Test one specific behavior per test method
- Include tests for:
  - Null checks on all nullable parameters
  - Boundary conditions (empty collections, max values, min values)
  - Exception scenarios with proper exception type verification
  - Validation failures with appropriate error messages
- Generate both positive test cases (success) and negative test cases (failures)
- Avoid testing implementation details; focus on behavior and contracts
- Use meaningful assertion messages that explain what went wrong
</testing_standards>

## MongoDB Best Practices

<mongodb_guidelines>
- Use strongly-typed filters: `Builders<T>.Filter.Eq(x => x.Property, value)`
- Implement proper projection to reduce data transfer
- Use indexes for fields that are frequently queried or sorted
- Handle concurrent updates with optimistic concurrency when needed
- Use transactions for multi-document operations when consistency is critical
- Properly handle MongoDB exceptions and connection issues
</mongodb_guidelines>

## Verification Checklist

<verification_checklist>
**Build and Tests**:
- [ ] `dotnet build DarkDeeds.sln -c Release` passes with no warnings
- [ ] `dotnet test DarkDeeds.sln -c Release` passes all tests
- [ ] No new compiler warnings introduced

**Backend-Specific Checks**:
- [ ] HTTP status codes are appropriate (200/201/204/400/401/403/404/500)
- [ ] Response models and input validation are comprehensive
- [ ] Error responses are consistent and informative
- [ ] MongoDB queries are efficient (use projection, avoid N+1)
- [ ] Proper logging at appropriate levels (Debug, Info, Warning, Error)
- [ ] No hardcoded values; use configuration or constants
</verification_checklist>
