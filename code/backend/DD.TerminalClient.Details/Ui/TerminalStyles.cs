using DD.TerminalClient.Domain.Models;
using Spectre.Console;

namespace DD.TerminalClient.Details.Ui;

// The single source of terminal frame styling. Task styling mirrors the web DayCardItem rules: a focused
// task inverts foreground and background; a completed task is struck through and dimmed; a probable task
// is italic; and the task type tints the text (Additional/Routine/Weekly are secondary greys, Routine
// dimmed and Weekly bold to stay distinguishable). Chrome styles cover day headers, today, section
// titles and hints. Colours are kept to the 16-colour palette so they render on a basic SSH terminal.
internal static class TerminalStyles
{
    public static readonly Style DayHeader = new(Color.Silver, decoration: Decoration.Bold);

    public static readonly Style Today = new(Color.Yellow, decoration: Decoration.Bold);

    public static readonly Style SectionTitle = new(Color.Teal, decoration: Decoration.Bold);

    public static readonly Style Hint = new(Color.Grey);

    public static readonly Style EmptyState = new(Color.Grey, decoration: Decoration.Italic);

    // Composes the style for one rendered task line from its type, completion, probability and focus.
    public static Style ForTask(TerminalTask task, bool isSelected)
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

        if (isSelected)
        {
            return new Style(Color.Black, Color.Silver, decoration);
        }

        if (task.Type == TerminalTaskType.Routine)
        {
            decoration |= Decoration.Dim;
        }

        if (task.Type == TerminalTaskType.Weekly)
        {
            decoration |= Decoration.Bold;
        }

        var foreground = task.Completed || task.Type != TerminalTaskType.Simple
            ? Color.Grey
            : Color.Default;

        return new Style(foreground, decoration: decoration);
    }
}
