namespace DD.TerminalClient.Details.Ui;

// The interactive UI mode the frame renders its status area for. It is a rendering-only view of the
// application mode owned by the event loop: the input reducer (a later iteration) decides transitions,
// while the renderer only reflects the current mode. Kept independent of the Domain input types so the
// frame can be built and tested before the reducer exists.
public enum TerminalStatusKind
{
    Normal,
    Editor,
    Login,
    Confirmation,
    Help,
}
