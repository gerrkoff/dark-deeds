---
applyTo: 'code/frontend/**'
---
# React Frontend Development Rules
You are an expert React and TypeScript developer. Generate modern, production-ready React components following these guidelines:

## Component Structure

<component_standards>
- Use functional components with hooks exclusively
- Implement proper TypeScript interfaces for props with clear, descriptive names
- Include default props when appropriate for optional properties
- Export components as both default and named exports for flexibility
- Keep components focused and single-responsibility
- Extract complex logic into custom hooks
- Use meaningful component and prop names that express intent
</component_standards>

## TypeScript Best Practices

<typescript_guidelines>
- Define explicit types for all props, state, and return values
- Use interfaces for props and object shapes
- Prefer union types over enums for string literals when appropriate
- Avoid `any` type; use `unknown` if type is truly unknown
- Use generics for reusable component patterns
- Leverage type inference where it improves readability
</typescript_guidelines>

## Code Quality Standards

<code_quality>
- Follow React best practices and naming conventions
- Use proper state management (useState, useEffect, useReducer, custom hooks)
- Implement proper error boundaries for component trees that might error
- Add JSDoc comments for complex props, functions, and custom hooks
- Use proper accessibility attributes (ARIA roles, labels, semantic HTML)
- Implement proper form validation and user feedback
- Handle loading states, error states, and empty states explicitly
</code_quality>

## Styling and UI

<styling_guidelines>
- Use CSS modules or styled-components for component-scoped styling
- Implement responsive design principles with mobile-first approach
- Follow Material-UI, Ant Design, or project-specific design system patterns
- Maintain consistent spacing, typography, and color usage
- Include proper hover, focus, and active states for interactive elements
- Use CSS variables for theme-able values
- Optimize for visual hierarchy and readability
</styling_guidelines>

## Performance Optimization

<performance>
- Use React.memo for components that receive stable props
- Implement proper dependency arrays in useEffect to avoid unnecessary re-renders
- Use useCallback for functions passed as props to memoized components
- Use useMemo for expensive computations
- Implement code splitting with React.lazy and Suspense for large components
- Avoid creating functions or objects in render that cause child re-renders
- Use virtualization for long lists (react-window, react-virtualized)
</performance>

## State Management

<state_management>
- Keep state as local as possible; lift only when necessary
- Use Context API for truly global or deeply nested state
- Consider using custom hooks to encapsulate complex state logic
- Avoid prop drilling by using composition or Context
- Use reducers (useReducer) for complex state with multiple sub-values
- Implement proper loading and error states in async operations
</state_management>

## Accessibility Requirements

<accessibility>
- Use semantic HTML elements (button, nav, main, article, etc.)
- Implement proper keyboard navigation (tab order, focus management)
- Include ARIA labels and roles where semantic HTML is insufficient
- Ensure proper color contrast ratios
- Provide text alternatives for images and icons
- Test with screen readers and keyboard-only navigation
- Handle focus management in modals and dynamic content
</accessibility>

## Testing Considerations

<testing_standards>
### Test Structure
- Use React Testing Library for component tests
- Focus on testing user behavior, not implementation details
- Test accessibility features (roles, labels, keyboard interactions)
- Use descriptive test names: `should do X when Y happens`

### Test Coverage
- User interactions (clicks, form inputs, navigation)
- Conditional rendering based on props or state
- Loading states, error states, and empty states
- Async operations and side effects
- Edge cases (empty data, long strings, special characters)
- Accessibility (keyboard navigation, screen reader support)

### Test Quality
- Tests should be isolated and not depend on each other
- Mock external dependencies (API calls, external libraries)
- Use data-testid sparingly; prefer queries by role, label, or text
- Ensure tests are maintainable and readable
</testing_standards>

## React Hooks Best Practices

<hooks_guidelines>
- Follow the Rules of Hooks (only at top level, only in React functions)
- Use useEffect only for side effects, not for derived state
- Clean up side effects properly (return cleanup function from useEffect)
- Use useCallback and useMemo judiciously (only when performance matters)
- Create custom hooks for reusable stateful logic
- Name custom hooks starting with "use" prefix
- Keep hooks focused and single-purpose
</hooks_guidelines>

## Verification Checklist

<verification_checklist>
**Build and Lint**:
- [ ] `npm run build` passes with no errors or warnings
- [ ] `npm run fmt:check` passes (or `npm run fmt` was run)
- [ ] `npm run test:run` passes all tests
- [ ] No TypeScript errors

**Frontend-Specific Checks**:
- [ ] TypeScript types are properly defined (no `any` types)
- [ ] Components are properly memoized if needed
- [ ] No console.log or debugging code left in
- [ ] Semantic HTML is used where appropriate
- [ ] ARIA labels are present for non-semantic elements
- [ ] Keyboard navigation works (Tab, Enter, Escape)
- [ ] Responsive design works on mobile, tablet, and desktop
- [ ] Visual consistency with design system
- [ ] Hover and focus states are visible
</verification_checklist>
