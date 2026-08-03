namespace DD.TerminalClient.Domain.Navigation;

// The four spatial moves the terminal supports in normal mode, driven by the arrow keys and their
// h/j/k/l aliases. Navigation translates a direction plus the current focus into the next focus.
public enum NavigationDirection
{
    Up,
    Down,
    Left,
    Right,
}
