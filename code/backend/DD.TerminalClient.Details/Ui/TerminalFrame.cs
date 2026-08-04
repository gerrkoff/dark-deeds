using System.Globalization;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Overview;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DD.TerminalClient.Details.Ui;

// Assembles the full terminal frame: a fixed one-line header, the scrollable Overview (or the help
// keymap, or an empty-state message), and a fixed status footer. The footer reflects the current mode
// without ever running a Spectre prompt inside the live display: it draws the supplied editor/login
// buffer and cursor as ordinary styled text, shows the delete confirmation question, the full focused
// task text in Normal mode, and the offline and conflict notifications. All user-supplied text (titles,
// profile name, notifications, editor buffer) is escaped so it can never be interpreted as markup.
public static class TerminalFrame
{
    public static IRenderable Render(TerminalViewModel model, int width)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new Rows(RenderHeader(model), RenderContent(model, width), RenderFooter(model));
    }

    public static IRenderable RenderHeader(TerminalViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var left = "[bold]Dark Deeds[/]";
        if (!string.IsNullOrEmpty(model.ProfileName))
        {
            left += " [grey]" + Markup.Escape(model.ProfileName) + "[/]";
        }

        var right = new List<string>();
        if (model.ShowCompleted)
        {
            right.Add("[grey]completed shown[/]");
        }

        right.Add(model.IsOffline ? "[red bold]offline[/]" : "[green]online[/]");

        var grid = new Grid { Expand = true };
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddColumn(new GridColumn().NoWrap().RightAligned());
        grid.AddRow(new Markup(left), new Markup(string.Join("  ", right)));
        return grid;
    }

    public static IRenderable RenderContent(TerminalViewModel model, int width)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.Status.Kind == TerminalStatusKind.Help)
        {
            return RenderHelp();
        }

        var overview = OverviewRenderer.Render(model, width);
        if (overview.Lines.Count == 0)
        {
            return RenderEmptyState();
        }

        return new Rows(overview.Lines);
    }

    public static IRenderable RenderFooter(TerminalViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var body = new List<IRenderable>();

        if (model.IsOffline)
        {
            body.Add(new Markup("[red bold]OFFLINE[/] [grey]showing cached tasks[/]"));
        }

        if (model.Notification is { } notification && !string.IsNullOrWhiteSpace(notification))
        {
            body.Add(new Markup("[yellow bold]" + Markup.Escape(TerminalText.Sanitize(notification)) + "[/]"));
        }

        var status = model.Status;
        switch (status.Kind)
        {
            case TerminalStatusKind.Editor:
                body.Add(BuildInputLine(status, mask: false));
                body.Add(HintLine(string.IsNullOrEmpty(status.Hint) ? "Enter save   Esc cancel" : status.Hint));
                break;
            case TerminalStatusKind.Login:
                body.Add(BuildInputLine(status, mask: status.MaskInput));
                body.Add(HintLine(string.IsNullOrEmpty(status.Hint) ? "Enter submit   Esc cancel" : status.Hint));
                break;
            case TerminalStatusKind.Confirmation:
                body.Add(new Markup("[bold]" + Markup.Escape(TerminalText.Sanitize(status.Prompt)) + "[/]"));
                body.Add(HintLine("y confirm   n / Esc cancel"));
                break;
            case TerminalStatusKind.Help:
                body.Add(HintLine("? or Esc to close help"));
                break;
            default:
                body.Add(BuildSelectedLine(model));
                body.Add(HintLine("a add   e edit   space done   d delete   m move   ? help   q quit"));
                break;
        }

        return new Panel(new Rows(body))
            .Expand()
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey);
    }

    private static Panel RenderHelp()
    {
        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap().PadRight(3));
        grid.AddColumn();
        AddHelpRow(grid, "Up/Down, k j", "Previous / next task");
        AddHelpRow(grid, "Left/Right, h l", "Previous / next day");
        AddHelpRow(grid, "a", "Add a task on the focused day");
        AddHelpRow(grid, "A", "Add a No Date task");
        AddHelpRow(grid, "e", "Edit the focused task");
        AddHelpRow(grid, "Space", "Toggle completed");
        AddHelpRow(grid, "d", "Delete (with confirmation)");
        AddHelpRow(grid, "Shift+Up/Down, K J", "Reorder within the day");
        AddHelpRow(grid, "Shift+Left/Right, H L", "Move one day");
        AddHelpRow(grid, "m", "Move to an explicit date");
        AddHelpRow(grid, "r", "Toggle Routine visibility for all days");
        AddHelpRow(grid, "c", "Toggle completed visibility");
        AddHelpRow(grid, "Ctrl+R", "Reconnect and reload");
        AddHelpRow(grid, "?", "Toggle this help");
        AddHelpRow(grid, "q", "Quit");

        return new Panel(grid)
            .Header(" Keyboard shortcuts ")
            .Expand()
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey);
    }

    private static void AddHelpRow(Grid grid, string keys, string action)
    {
        grid.AddRow(new Markup("[bold]" + Markup.Escape(keys) + "[/]"), new Text(action));
    }

    private static Markup RenderEmptyState()
    {
        return new Markup("[grey italic]No tasks to show. Press 'a' to add one.[/]");
    }

    private static Markup BuildInputLine(TerminalStatus status, bool mask)
    {
        var shown = mask
            ? new string('*', status.Input.Length)
            : TerminalText.Sanitize(status.Input);
        var cursor = Math.Clamp(status.CursorPosition, 0, shown.Length);
        var before = Markup.Escape(shown[..cursor]);
        var atCursor = cursor < shown.Length ? shown[cursor].ToString() : " ";
        var after = cursor < shown.Length ? Markup.Escape(shown[(cursor + 1)..]) : string.Empty;
        var prompt = string.IsNullOrEmpty(status.Prompt)
            ? string.Empty
            : "[bold]" + Markup.Escape(status.Prompt) + "[/] ";
        return new Markup(prompt + before + "[invert]" + Markup.Escape(atCursor) + "[/]" + after);
    }

    private static Markup BuildSelectedLine(TerminalViewModel model)
    {
        var task = FindFocusedTask(model);
        if (task is null)
        {
            return new Markup("[grey italic]No task selected[/]");
        }

        return new Markup("[bold]Selected:[/] " + Markup.Escape(BuildSelectedText(task)));
    }

    private static Markup HintLine(string text)
    {
        return new Markup("[grey]" + Markup.Escape(text) + "[/]");
    }

    private static TerminalTask? FindFocusedTask(TerminalViewModel model)
    {
        var uid = model.Focus?.Uid;
        if (uid is null)
        {
            return null;
        }

        foreach (var task in EnumerateTasks(model.Overview))
        {
            if (string.Equals(task.Uid, uid, StringComparison.Ordinal))
            {
                return task;
            }
        }

        return null;
    }

    private static IEnumerable<TerminalTask> EnumerateTasks(OverviewProjection overview)
    {
        var days = new List<OverviewDay> { overview.NoDate };
        days.AddRange(overview.Overdue);
        days.AddRange(overview.Current);
        days.AddRange(overview.Future);

        foreach (var day in days)
        {
            foreach (var overviewTask in day.Tasks)
            {
                yield return overviewTask.Task;
            }
        }
    }

    private static string BuildSelectedText(TerminalTask task)
    {
        var parts = new List<string>();
        if (task.Date is { } date)
        {
            parts.Add(date.ToString("ddd dd MMM", CultureInfo.InvariantCulture));
        }

        if (task.Time is { } minutes)
        {
            parts.Add(TerminalText.FormatTime(minutes));
        }

        parts.Add(task.Title);
        if (task.IsProbable)
        {
            parts.Add("(probable)");
        }

        if (task.Type != TerminalTaskType.Simple)
        {
            parts.Add("(" + task.Type.ToString() + ")");
        }

        return TerminalText.Sanitize(string.Join(" ", parts));
    }
}
