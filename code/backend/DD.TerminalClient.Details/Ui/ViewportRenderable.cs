using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DD.TerminalClient.Details.Ui;

// A Spectre renderable that turns an arbitrary content renderable into a fixed-height scrollable viewport.
// It renders the inner content once, splits it into single terminal lines with Segment.SplitLines so every
// segment keeps its style, then emits exactly ViewportState.ViewportHeight lines: an optional "more above"
// indicator, the clipped window of content lines starting at ViewportState.Offset, an optional "more below"
// indicator, and blank padding so the surrounding fixed header and footer never move. Because the Overview
// renders one line per task, clipping by line keeps each focused task's complete line on screen. The type
// also resolves the terminal dimensions, falling back to Spectre profile capabilities when the operating
// system window-size API is unavailable, and renders the resize-required screen shown below the minimum
// size, where synchronization keeps running and only '?' and 'q' are accepted.
public sealed class ViewportRenderable : Renderable
{
    private readonly IRenderable _content;
    private readonly ViewportState _state;

    public ViewportRenderable(IRenderable content, ViewportState state)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(state);
        _content = content;
        _state = state;
    }

    // Resolves the current terminal width and height, preferring the operating system window size and
    // falling back to the Spectre profile capabilities for any dimension that is unavailable (windowSize
    // returns null) or nonpositive, which happens when output is redirected or the window-size API is
    // unsupported. This lets the client keep a usable layout instead of failing on a missing API.
    public static (int Width, int Height) ResolveDimensions(
        IAnsiConsole console,
        Func<(int Width, int Height)?> windowSize)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(windowSize);

        var os = windowSize();
        var width = os is { Width: > 0 } ? os.Value.Width : console.Profile.Width;
        var height = os is { Height: > 0 } ? os.Value.Height : console.Profile.Height;
        return (width, height);
    }

    // Reads the operating system window size, returning null when the API is unavailable (redirected
    // output or an unsupported platform) so ResolveDimensions can fall back to the profile capabilities.
    public static (int Width, int Height)? ReadWindowSize()
    {
        try
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;
            return width > 0 && height > 0 ? (width, height) : null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (PlatformNotSupportedException)
        {
            return null;
        }
    }

    // Maps the focused task to the content line the viewport should scroll into view. When moving back
    // up to the first task of a card whose header is above the current window, the preceding header line
    // becomes the scroll anchor so labels such as No Date reappear. Otherwise the task line itself stays
    // the anchor, ensuring the focused task is never clipped below the window.
    public static int? FindFocusedLine(RenderedOverview rendered, TaskFocus? focus, int previousOffset = 0)
    {
        ArgumentNullException.ThrowIfNull(rendered);
        if (focus is null)
        {
            return null;
        }

        foreach (var line in rendered.TaskLines)
        {
            if (line.Address == focus.Address)
            {
                if (line.Address.TaskIndex == 0 && previousOffset >= line.LineIndex)
                {
                    return Math.Max(0, line.LineIndex - 1);
                }

                return line.LineIndex;
            }
        }

        return null;
    }

    // Builds the resize-required screen shown when the terminal is smaller than the supported minimum. It
    // states the current and required sizes and that synchronization keeps running while only '?' and 'q'
    // are accepted, so the user knows the client is still live while waiting for a larger window.
    public static IRenderable RenderResizeRequired(int width, int height)
    {
        var body = new Grid();
        body.AddColumn();
        body.AddRow(new Markup("[yellow bold]Terminal too small[/]"));
        body.AddRow(new Text(
            $"Current size {width}x{height}. Dark Deeds needs at least "
            + $"{ViewportState.MinimumWidth}x{ViewportState.MinimumHeight}."));
        body.AddRow(new Markup("[grey]Resize the window to continue. Syncing continues in the background.[/]"));
        body.AddRow(new Markup("[grey]? help   q quit[/]"));

        return new Panel(body)
            .Header(" Resize required ")
            .Expand()
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Yellow);
    }

    protected override Measurement Measure(RenderOptions options, int maxWidth)
    {
        return _content.Measure(options, maxWidth);
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var lines = Segment.SplitLines(_content.Render(options, maxWidth));
        var output = new List<Segment>();
        var emitted = 0;

        void EmitLine(IEnumerable<Segment> segments)
        {
            output.AddRange(segments);
            output.Add(Segment.LineBreak);
            emitted++;
        }

        if (!_state.Scrollable)
        {
            for (var i = 0; i < lines.Count && emitted < _state.ViewportHeight; i++)
            {
                EmitLine(lines[i]);
            }

            while (emitted < _state.ViewportHeight)
            {
                EmitLine([]);
            }

            return output;
        }

        EmitLine(_state.HasAbove ? IndicatorSegments(_state.Offset, above: true, options, maxWidth) : []);

        var start = Math.Clamp(_state.Offset, 0, lines.Count);
        var end = Math.Min(lines.Count, start + _state.WindowHeight);
        for (var i = start; i < end; i++)
        {
            EmitLine(lines[i]);
        }

        while (emitted < _state.ViewportHeight - 1)
        {
            EmitLine([]);
        }

        var hiddenBelow = Math.Max(0, _state.TotalLines - (_state.Offset + _state.WindowHeight));
        EmitLine(_state.HasBelow ? IndicatorSegments(hiddenBelow, above: false, options, maxWidth) : []);
        return output;
    }

    // Renders a continuation indicator line ("N more above"/"N more below") to a fresh list of segments.
    // The line is rendered through Spectre's Text so its style matches the rest of the chrome, then copied
    // with AddRange rather than a collection-expression spread, which would introduce null padding segments.
    private static List<Segment> IndicatorSegments(int hiddenCount, bool above, RenderOptions options, int maxWidth)
    {
        var where = above ? "above" : "below";
        var text = TerminalText.Truncate($"  {hiddenCount} more {where}", maxWidth);
        var segments = ((IRenderable)new Text(text, TerminalStyles.Hint)).Render(options, maxWidth);
        var rendered = Segment.SplitLines(segments);
        var line = new List<Segment>();
        if (rendered.Count > 0)
        {
            line.AddRange(rendered[0]);
        }

        return line;
    }
}
