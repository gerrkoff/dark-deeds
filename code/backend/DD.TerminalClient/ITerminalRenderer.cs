using DD.TerminalClient.Details.Ui;

namespace DD.TerminalClient;

// The seam between the event loop and the terminal surface. Begin enters the alternate screen and hides
// the cursor once at startup; Render draws one coalesced frame per drained event batch; End restores the
// cursor and leaves the alternate screen in the loop's finally. Isolating the surface lets tests drive
// the whole application against a recording fake without touching a real terminal or Spectre live display.
internal interface ITerminalRenderer
{
    void Begin();

    void Render(TerminalViewModel model, int width, int height, bool resizeRequired);

    void End();
}
