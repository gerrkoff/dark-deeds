using System.Globalization;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Overview;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DD.TerminalClient.Details.Ui;

// Renders the Overview projection into a flat, top-to-bottom list of single-line renderables plus the
// per-task line metadata the viewport consumes. Every visible task becomes exactly one ellipsized line,
// so its VisualTaskAddress maps deterministically to a content line index regardless of title length.
// Section titles, day headers and the collapsed-Routine summary each also occupy exactly one line. User
// titles are sanitized, cell-width truncated and rendered as plain styled Text, never parsed as markup,
// so a title containing "[" or Spectre tags can neither break the layout nor inject styling.
public static class OverviewRenderer
{
    public static RenderedOverview Render(TerminalViewModel model, int width)
    {
        ArgumentNullException.ThrowIfNull(model);

        var lines = new List<IRenderable>();
        var taskLines = new List<TaskLine>();
        var focusUid = model.Focus?.Uid;
        var overview = model.Overview;

        AppendNoDate(lines, taskLines, overview.NoDate, focusUid, width);
        AppendDatedSection(lines, taskLines, "Overdue", overview.Overdue, model.Today, focusUid, width);
        AppendDatedSection(lines, taskLines, "This week", overview.Current, model.Today, focusUid, width);
        AppendDatedSection(lines, taskLines, "Upcoming", overview.Future, model.Today, focusUid, width);

        return new RenderedOverview { Lines = lines, TaskLines = taskLines };
    }

    private static void AppendNoDate(
        List<IRenderable> lines,
        List<TaskLine> taskLines,
        OverviewDay day,
        string? focusUid,
        int width)
    {
        if (day.Tasks.Count == 0)
        {
            return;
        }

        lines.Add(SectionTitleLine("No Date", width));
        AppendTasks(lines, taskLines, day, focusUid, width);
    }

    private static void AppendDatedSection(
        List<IRenderable> lines,
        List<TaskLine> taskLines,
        string title,
        IReadOnlyList<OverviewDay> days,
        DateOnly today,
        string? focusUid,
        int width)
    {
        var visibleDays = new List<OverviewDay>();
        foreach (var day in days)
        {
            if (day.Tasks.Count > 0 || day.CollapsedRoutineCount > 0)
            {
                visibleDays.Add(day);
            }
        }

        if (visibleDays.Count == 0)
        {
            return;
        }

        lines.Add(SectionTitleLine(title, width));
        foreach (var day in visibleDays)
        {
            lines.Add(DayHeaderLine(day, today, width));
            AppendTasks(lines, taskLines, day, focusUid, width);
        }
    }

    private static void AppendTasks(
        List<IRenderable> lines,
        List<TaskLine> taskLines,
        OverviewDay day,
        string? focusUid,
        int width)
    {
        foreach (var overviewTask in day.Tasks)
        {
            var isSelected = focusUid is not null
                && string.Equals(overviewTask.Task.Uid, focusUid, StringComparison.Ordinal);
            taskLines.Add(new TaskLine { Address = overviewTask.Address, LineIndex = lines.Count });
            lines.Add(TaskLineRenderable(overviewTask.Task, isSelected, width));
        }

        if (day.CollapsedRoutineCount > 0)
        {
            lines.Add(CollapsedRoutineLine(day.CollapsedRoutineCount, width));
        }
    }

    private static Text TaskLineRenderable(TerminalTask task, bool isSelected, int width)
    {
        var marker = isSelected ? "> " : "  ";
        var time = task.Time is { } minutes ? TerminalText.FormatTime(minutes) + " " : string.Empty;
        var prefix = marker + time;
        var titleBudget = Math.Max(0, width - prefix.GetCellWidth());
        var line = prefix + TerminalText.Truncate(task.Title, titleBudget);
        return new Text(line, TerminalStyles.ForTask(task, isSelected)) { Overflow = Overflow.Ellipsis };
    }

    private static Text SectionTitleLine(string title, int width)
    {
        return new Text(TerminalText.Truncate("== " + title + " ==", width), TerminalStyles.SectionTitle);
    }

    private static Text DayHeaderLine(OverviewDay day, DateOnly today, int width)
    {
        var label = day.Date is { } date
            ? date.ToString("ddd dd MMM", CultureInfo.InvariantCulture)
            : "No Date";
        var isToday = day.Date is { } value && value == today;
        var style = isToday ? TerminalStyles.Today : TerminalStyles.DayHeader;
        return new Text("  " + TerminalText.Truncate(label, Math.Max(0, width - 2)), style);
    }

    private static Text CollapsedRoutineLine(int count, int width)
    {
        var label = "    +" + count.ToString(CultureInfo.InvariantCulture) + " routine";
        return new Text(TerminalText.Truncate(label, width), TerminalStyles.Hint);
    }
}
