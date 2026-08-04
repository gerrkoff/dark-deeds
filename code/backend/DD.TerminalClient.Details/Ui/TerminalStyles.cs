using DD.TerminalClient.Domain.Models;
using Spectre.Console;

namespace DD.TerminalClient.Details.Ui;

// The single source of terminal frame styling. Selection is represented by the task-line marker rather
// than title highlighting. Completed tasks are struck through and dimmed, probable tasks are italic,
// Additional tasks are dimmed, and every non-Simple type uses the secondary grey foreground. Chrome
// styles cover day headers, today, section titles and hints. Colours stay within the 16-colour palette.
internal static class TerminalStyles
{
    public static readonly Style DayHeader = new(Color.Blue, decoration: Decoration.Bold);

    public static readonly Style Today = new(Color.Yellow, decoration: Decoration.Bold);

    public static readonly Style SectionTitle = new(Color.Teal, decoration: Decoration.Bold);

    public static readonly Style Hint = new(Color.Grey);

    public static readonly Style EmptyState = new(Color.Grey, decoration: Decoration.Italic);

    // Composes the style for one rendered task line from its type, completion, probability and focus.
    public static Style ForTask(TerminalTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var decoration = Decoration.None;

        if (task.Completed)
        {
            decoration |= Decoration.Strikethrough;
        }

        if (task.IsProbable)
        {
            decoration |= Decoration.Italic;
        }

        if (task.Type == TerminalTaskType.Additional || task.Completed)
        {
            decoration |= Decoration.Dim;
        }

        var foreground = task.Completed || task.Type != TerminalTaskType.Simple
            ? Color.Grey
            : Color.Default;

        return new Style(foreground, decoration: decoration);
    }
}
