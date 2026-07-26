namespace DD.TerminalClient.Domain.Input;

// The intent the reducer emits for the application event loop to act on. It is deliberately free of any
// payload: navigation, reorder, day-move, the toggles, reconnect and quit act on the current focus the
// application already owns, while the Submit* commands carry the committed editor text on the result and
// ConfirmDelete acts on the focused task. None means the key produced no application-visible action (it
// may still have changed the input mode or the editor buffer, both carried on the result state).
public enum TerminalCommand
{
    None,
    NavigateUp,
    NavigateDown,
    NavigateLeft,
    NavigateRight,
    ReorderUp,
    ReorderDown,
    MoveDayBackward,
    MoveDayForward,
    ToggleComplete,
    ToggleCompletedVisibility,
    ToggleRoutine,
    ForceReconnect,
    Quit,
    SubmitAddWithFocusDate,
    SubmitAddNoDate,
    SubmitEdit,
    SubmitMove,
    SubmitLogin,
    ConfirmDelete,
}
