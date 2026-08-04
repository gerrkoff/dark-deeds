using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Tasks;

// Ports the React TaskSaveService.getTasksToSync. Given the last-known tasks and a set of edited
// tasks, it returns the minimal set to persist, renumbering every affected date group (both the
// source a task left and the destination it entered) to a contiguous 1..N order and preserving a
// higher already-saved version. No Date is its own group. Result order matches the web Map
// semantics: edited tasks first (in input order), then any task pulled in only by renumbering.
public static class TaskOrderService
{
    private const int NoDateKey = -1;

    public static IReadOnlyList<TerminalTask> GetTasksToSync(
        IReadOnlyList<TerminalTask> tasks,
        IReadOnlyList<TerminalTask> updatedTasks)
    {
        var baseByUid = new Dictionary<string, TerminalTask>();
        foreach (var task in tasks)
            baseByUid[task.Uid] = task;

        // Insertion-ordered map mirroring the JS Map: re-setting an existing uid keeps its position,
        // a new uid is appended.
        var syncOrder = new List<string>();
        var sync = new Dictionary<string, TerminalTask>();

        void SetSync(TerminalTask task)
        {
            if (!sync.ContainsKey(task.Uid))
                syncOrder.Add(task.Uid);
            sync[task.Uid] = task;
        }

        foreach (var task in updatedTasks)
            SetSync(FixVersion(task, baseByUid));

        var tasksByDate = new Dictionary<int, List<TerminalTask>>();
        var affectedDates = new List<int>();
        var affectedDatesSet = new HashSet<int>();

        List<TerminalTask> TasksOnDate(int dateKey)
        {
            if (!tasksByDate.TryGetValue(dateKey, out var list))
            {
                list = [];
                tasksByDate[dateKey] = list;
            }

            return list;
        }

        void MarkAffected(int dateKey)
        {
            if (affectedDatesSet.Add(dateKey))
                affectedDates.Add(dateKey);
        }

        foreach (var task in updatedTasks)
        {
            TasksOnDate(DateKey(task)).Add(task);
            MarkAffected(DateKey(task));
        }

        foreach (var task in tasks)
        {
            if (!sync.ContainsKey(task.Uid))
                TasksOnDate(DateKey(task)).Add(task);
            else
                MarkAffected(DateKey(task));
        }

        foreach (var dateKey in affectedDates)
        {
            var onDate = TasksOnDate(dateKey).OrderBy(task => task.Order).ToList();

            for (var i = 0; i < onDate.Count; i++)
            {
                var task = onDate[i];
                var order = i + 1;
                if (task.Order == order)
                    continue;

                if (sync.TryGetValue(task.Uid, out var pending))
                    sync[task.Uid] = pending with { Order = order };
                else
                    SetSync(FixVersion(task with { Order = order }, baseByUid));
            }
        }

        return [.. syncOrder.Select(uid => sync[uid])];
    }

    private static int DateKey(TerminalTask task)
    {
        return task.Date is { } date ? date.DayNumber : NoDateKey;
    }

    // Preserves a version already advanced by a prior save when the same task is re-edited: if the
    // last-known task carries a higher version than the incoming edit, keep the higher version.
    private static TerminalTask FixVersion(TerminalTask task, Dictionary<string, TerminalTask> baseByUid)
    {
        return baseByUid.TryGetValue(task.Uid, out var existing) && existing.Version > task.Version
            ? task with { Version = existing.Version }
            : task;
    }
}
