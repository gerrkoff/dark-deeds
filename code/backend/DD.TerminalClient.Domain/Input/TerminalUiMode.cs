namespace DD.TerminalClient.Domain.Input;

// The interactive input mode that decides how a key press is interpreted. Only Normal maps keys to the
// task keymap; every other mode either edits a text buffer or answers a single prompt, so text typed in
// an editor or a confirmation can never trigger a normal-mode command. ResizeRequired is entered by the
// application when the terminal drops below the supported minimum: synchronization keeps running while
// the reducer accepts only the help and quit keys.
public enum TerminalUiMode
{
    Normal,
    Editor,
    MaskedLogin,
    DeleteConfirmation,
    Help,
    ResizeRequired,
}
