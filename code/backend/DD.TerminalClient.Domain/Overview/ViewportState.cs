namespace DD.TerminalClient.Domain.Overview;

// The pure vertical-scroll model for the Overview content area, free of any Spectre, terminal or
// file-system dependency. Given the number of one-line content rows, the height reserved for content
// (the terminal height minus the fixed header and footer), the focused row and the previous offset, it
// computes the scroll Offset that keeps the focused row's complete line visible, the WindowHeight of
// content actually shown, and whether hidden rows remain above or below. When the content fits, it does
// not scroll and reserves no indicator rows; when it does not fit, it reserves one continuation row above
// and one below so ViewportRenderable can draw the "more above"/"more below" markers without hiding a
// content line. IsResizeRequired reports when the terminal is below the supported minimum, in which case
// the loop keeps synchronizing while the resize-required screen is shown instead of the viewport.
public sealed record ViewportState
{
    public const int MinimumWidth = 120;

    public const int MinimumHeight = 30;

    private const int IndicatorReserve = 2;

    public required int Offset { get; init; }

    public required int WindowHeight { get; init; }

    public required int ViewportHeight { get; init; }

    public required int TotalLines { get; init; }

    public required bool Scrollable { get; init; }

    public required bool HasAbove { get; init; }

    public required bool HasBelow { get; init; }

    // True when the terminal is too small to render the full frame, so the caller shows resize-required
    // mode instead of the viewport while keeping synchronization running.
    public static bool IsResizeRequired(int width, int height)
    {
        return width < MinimumWidth || height < MinimumHeight;
    }

    // Reserves the fixed header/footer height: the number of content rows available for the viewport is
    // the terminal height minus the reserved chrome rows, never below one.
    public static int ContentHeight(int terminalHeight, int reservedRows)
    {
        return Math.Max(1, terminalHeight - reservedRows);
    }

    // Computes the viewport for the given content. focusedLine is the zero-based content line the focused
    // task occupies (null when nothing is focused); previousOffset preserves the scroll position across
    // renders so unrelated updates do not jump the view. The focused line is always placed inside the
    // visible window, so its complete single line stays on screen without any independent scrolling.
    public static ViewportState Calculate(int totalLines, int viewportHeight, int? focusedLine, int previousOffset)
    {
        var lines = Math.Max(0, totalLines);
        var height = Math.Max(1, viewportHeight);

        if (lines <= height)
        {
            return new ViewportState
            {
                Offset = 0,
                WindowHeight = lines,
                ViewportHeight = height,
                TotalLines = lines,
                Scrollable = false,
                HasAbove = false,
                HasBelow = false,
            };
        }

        var window = Math.Max(1, height - IndicatorReserve);
        var maxOffset = lines - window;
        var offset = Math.Clamp(previousOffset, 0, maxOffset);

        if (focusedLine is { } focus)
        {
            var target = Math.Clamp(focus, 0, lines - 1);
            if (target < offset)
            {
                offset = target;
            }
            else if (target >= offset + window)
            {
                offset = target - window + 1;
            }

            offset = Math.Clamp(offset, 0, maxOffset);
        }

        return new ViewportState
        {
            Offset = offset,
            WindowHeight = window,
            ViewportHeight = height,
            TotalLines = lines,
            Scrollable = true,
            HasAbove = offset > 0,
            HasBelow = offset + window < lines,
        };
    }
}
