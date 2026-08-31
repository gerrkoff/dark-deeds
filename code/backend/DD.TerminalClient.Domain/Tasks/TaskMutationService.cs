using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Tasks;

public enum ReorderDirection
{
    Up,
    Down,
}

// Pure, side-effect-free task edits. Each command returns the changed task(s); contiguous
// renumbering for persistence is left to TaskOrderService so a mutation never has to know a whole
// date group. New UIDs are supplied by the caller, keeping the Domain free of any identity or clock
// dependency and fully deterministic under test.
public static class TaskMutationService
{
    // A freshly created task is parked before No Date tasks or after dated tasks; TaskOrderService
    // then renumbers the affected group to a contiguous 1..N sequence.
    public const int PrependOrder = int.MinValue;
    public const int AppendOrder = int.MaxValue;

    public static TerminalTask Create(ParsedTaskText parsed, DateOnly? fallbackDate, string uid)
    {
        var date = parsed.Date ?? fallbackDate;

        return new TerminalTask
        {
            Uid = uid,
            Title = parsed.Title,
            Date = date,
            Time = parsed.Time,
            Order = date.HasValue ? AppendOrder : PrependOrder,
            Completed = false,
            Deleted = false,
            Type = parsed.Type.ToTerminalTaskType(),
            IsProbable = parsed.IsProbable,
            Version = 0,
        };
    }

    public static TerminalTask Edit(TerminalTask task, ParsedTaskText parsed)
    {
        return task with
        {
            Title = parsed.Title,
            Date = parsed.Date,
            Time = parsed.Time,
            Type = parsed.Type.ToTerminalTaskType(),
            IsProbable = parsed.IsProbable,
        };
    }

    public static TerminalTask ToggleCompleted(TerminalTask task)
    {
        return task with { Completed = !task.Completed };
    }

    public static TerminalTask Delete(TerminalTask task)
    {
        return task with { Deleted = true };
    }

    // Explicit move to a specific date or to No Date (null), used by the editor-driven move.
    public static TerminalTask Move(TerminalTask task, DateOnly? date)
    {
        return task with { Date = date };
    }

    // One calendar-day move. Unavailable for a No Date task until an explicit date is chosen, so it
    // returns null; the caller turns that into a status message instead of a mutation.
    public static TerminalTask? MoveByDays(TerminalTask task, int days)
    {
        return task.Date is { } date ? task with { Date = date.AddDays(days) } : null;
    }

    // Swaps the focused task with its nearest visible neighbor in the given direction, leaving every
    // hidden task's order untouched so hidden tasks keep their relative position. Returns the two
    // changed tasks (empty when there is no such neighbor); feed them to TaskOrderService to
    // renumber. The group may be in any order and may mix visible and hidden tasks.
    public static IReadOnlyList<TerminalTask> Reorder(
        IReadOnlyList<TerminalTask> group,
        string focusedUid,
        ISet<string> visibleUids,
        ReorderDirection direction)
    {
        var sorted = group.OrderBy(task => task.Order).ToList();
        var focusedIndex = sorted.FindIndex(task => task.Uid == focusedUid);
        if (focusedIndex < 0)
            return [];

        var step = direction == ReorderDirection.Up ? -1 : 1;
        var neighborIndex = -1;
        for (var i = focusedIndex + step; i >= 0 && i < sorted.Count; i += step)
        {
            if (visibleUids.Contains(sorted[i].Uid))
            {
                neighborIndex = i;
                break;
            }
        }

        if (neighborIndex < 0)
            return [];

        var focused = sorted[focusedIndex];
        var neighbor = sorted[neighborIndex];

        return
        [
            focused with { Order = neighbor.Order },
            neighbor with { Order = focused.Order },
        ];
    }
}
