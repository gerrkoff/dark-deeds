using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Overview;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DD.TerminalClient;

// The production terminal surface: an alternate-screen, full-frame renderer driven by the event loop.
// Begin switches to the alternate buffer and hides the cursor; End restores both. Each Render clears the
// buffer and writes a fixed header, the scrollable Overview (or help/empty content) clipped into a
// viewport that keeps the focused task visible, and a fixed footer - or the resize-required screen when
// the terminal is below the supported minimum. It never runs a Spectre prompt inside the live frame and
// never logs, so the alternate screen stays the sole owner of the console.
internal sealed class SpectreTerminalRenderer(IAnsiConsole console) : ITerminalRenderer
{
    private const string EnterAlternateScreen = "\u001b[?1049h";
    private const string LeaveAlternateScreen = "\u001b[?1049l";

    private int _offset;

    public void Begin()
    {
        console.Profile.Out.Writer.Write(EnterAlternateScreen);
        console.Profile.Out.Writer.Flush();
        console.Cursor.Hide();
        console.Clear();
    }

    public void Render(TerminalViewModel model, int width, int height, bool resizeRequired)
    {
        ArgumentNullException.ThrowIfNull(model);

        console.Clear();
        if (resizeRequired)
        {
            _offset = 0;
            console.Write(ViewportRenderable.RenderResizeRequired(width, height));
            return;
        }

        var header = TerminalFrame.RenderHeader(model);
        var footer = TerminalFrame.RenderFooter(model);
        var (content, contentLines, focusedLine) = BuildContent(model, width);

        var reserved = CountLines(header, width) + CountLines(footer, width);
        var contentHeight = ViewportState.ContentHeight(height, reserved);
        var viewport = ViewportState.Calculate(contentLines, contentHeight, focusedLine, _offset);
        _offset = viewport.Offset;

        console.Write(new Rows(header, new ViewportRenderable(content, viewport), footer));
    }

    public void End()
    {
        console.Cursor.Show();
        console.Profile.Out.Writer.Write(LeaveAlternateScreen);
        console.Profile.Out.Writer.Flush();
    }

    private (IRenderable Content, int Lines, int? FocusedLine) BuildContent(TerminalViewModel model, int width)
    {
        if (model.Status.Kind == TerminalStatusKind.Help)
        {
            var help = TerminalFrame.RenderContent(model, width);
            return (help, CountLines(help, width), null);
        }

        var rendered = OverviewRenderer.Render(model, width);
        if (rendered.Lines.Count == 0)
        {
            var empty = TerminalFrame.RenderContent(model, width);
            return (empty, CountLines(empty, width), null);
        }

        var focusedLine = ViewportRenderable.FindFocusedLine(rendered, model.Focus);
        return (new Rows(rendered.Lines), rendered.Lines.Count, focusedLine);
    }

    private int CountLines(IRenderable renderable, int width)
    {
        var options = RenderOptions.Create(console, console.Profile.Capabilities);
        return Segment.SplitLines(renderable.Render(options, width)).Count;
    }
}
