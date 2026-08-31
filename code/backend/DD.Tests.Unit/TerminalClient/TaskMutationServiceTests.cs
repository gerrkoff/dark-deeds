using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Tasks;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Ports code/frontend/tests/services/TaskSaveService.test.ts (getTasksToSync) to xUnit and adds
// version-preservation, cross-date source/destination renumbering, hidden-neighbor reorder and No
// Date cases, plus direct coverage of every pure TaskMutationService command.
public class TaskMutationServiceTests
{
    private static readonly DateOnly DateA = new(2026, 3, 10);
    private static readonly DateOnly DateB = new(2026, 3, 11);

    // --- Ported TaskSaveService.test.ts cases ---
    [Fact]
    public void GetTasksToSync_Empty_ReturnsEmpty()
    {
        var result = TaskOrderService.GetTasksToSync([], []);

        Assert.Empty(result);
    }

    [Fact]
    public void GetTasksToSync_AddTask_AssignsOrderOne()
    {
        var result = TaskOrderService.GetTasksToSync(
            [],
            [NewTask("1", date: DateA, order: 0)]);

        Assert.Single(result);
        Assert.Equal("1", result[0].Uid);
        Assert.Equal(1, result[0].Order);
    }

    [Fact]
    public void GetTasksToSync_AddNoDateTask_PrependsBeforeExistingTasks()
    {
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("existing", date: null, order: 1)],
            [NewTask("new", date: null, order: TaskMutationService.PrependOrder)]);

        var byUid = result.ToDictionary(task => task.Uid);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, byUid["new"].Order);
        Assert.Equal(2, byUid["existing"].Order);
    }

    [Fact]
    public void GetTasksToSync_UpdateTask_KeepsNewTitleAndVersion()
    {
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("1", date: DateA, order: 1, version: 0, title: "Old Title")],
            [NewTask("1", date: DateA, order: 1, version: 1, title: "New Title")]);

        Assert.Single(result);
        Assert.Equal("New Title", result[0].Title);
        Assert.Equal(1, result[0].Version);
    }

    [Fact]
    public void GetTasksToSync_DeleteTask_MarksDeleted()
    {
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("1", date: DateA, order: 1)],
            [NewTask("1", date: DateA, order: 1, version: 1, deleted: true)]);

        Assert.Single(result);
        Assert.True(result[0].Deleted);
    }

    [Fact]
    public void GetTasksToSync_ReorderTasks_SwapsOrders()
    {
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("1", date: DateA, order: 1), NewTask("2", date: DateA, order: 2)],
            [NewTask("2", date: DateA, order: 1, version: 1), NewTask("1", date: DateA, order: 2, version: 1)]);

        Assert.Equal(2, result.Count);
        Assert.Equal("2", result[0].Uid);
        Assert.Equal(1, result[0].Order);
        Assert.Equal("1", result[1].Uid);
        Assert.Equal(2, result[1].Order);
    }

    // --- Added cases ---
    [Fact]
    public void GetTasksToSync_PreservesHigherExistingVersion()
    {
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("1", date: DateA, order: 1, version: 2, title: "Old")],
            [NewTask("1", date: DateA, order: 1, version: 1, title: "New")]);

        Assert.Single(result);
        Assert.Equal("New", result[0].Title);
        Assert.Equal(2, result[0].Version);
    }

    [Fact]
    public void GetTasksToSync_MoveAcrossDates_RenumbersSourceAndDestination()
    {
        // A1 leaves DateA for the top of DateB (order 0): DateB pushes B1 down to 2 while A1 takes 1,
        // and DateA renumbers the surviving A2 from 2 down to 1.
        var result = TaskOrderService.GetTasksToSync(
            [
                NewTask("A1", date: DateA, order: 1),
                NewTask("A2", date: DateA, order: 2),
                NewTask("B1", date: DateB, order: 1),
            ],
            [NewTask("A1", date: DateB, order: 0, version: 1)]);

        var byUid = result.ToDictionary(task => task.Uid);
        Assert.Equal(3, result.Count);
        Assert.Equal(DateB, byUid["A1"].Date);
        Assert.Equal(1, byUid["A1"].Order);
        Assert.Equal(2, byUid["B1"].Order);
        Assert.Equal(1, byUid["A2"].Order);
    }

    [Fact]
    public void GetTasksToSync_NoDateGroup_RenumbersIndependently()
    {
        // Two No Date tasks with an order gap (1 and 3); editing the first renumbers the second to 2.
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("1", date: null, order: 1), NewTask("2", date: null, order: 3)],
            [NewTask("1", date: null, order: 1, version: 1, title: "Edited")]);

        var byUid = result.ToDictionary(task => task.Uid);
        Assert.Equal(2, result.Count);
        Assert.Equal("Edited", byUid["1"].Title);
        Assert.Equal(1, byUid["1"].Order);
        Assert.Equal(2, byUid["2"].Order);
    }

    [Fact]
    public void GetTasksToSync_NoDateAndDatedSameOrder_AreSeparateGroups()
    {
        // A dated task and a new No Date task share order value 1 but belong to separate groups; the
        // untouched dated task is not returned and the No Date task keeps order 1.
        var result = TaskOrderService.GetTasksToSync(
            [NewTask("dated", date: DateA, order: 1)],
            [NewTask("nodate", date: null, order: 1)]);

        Assert.Single(result);
        Assert.Equal("nodate", result[0].Uid);
        Assert.Equal(1, result[0].Order);
    }

    // --- Reorder (visible-neighbor) command ---
    [Fact]
    public void Reorder_Down_SwapsOrderWithNextVisibleNeighbor()
    {
        var group = new List<TerminalTask>
        {
            NewTask("A", date: DateA, order: 1),
            NewTask("B", date: DateA, order: 2),
        };

        var changed = TaskMutationService.Reorder(group, "A", Visible("A", "B"), ReorderDirection.Down);

        var byUid = changed.ToDictionary(task => task.Uid);
        Assert.Equal(2, changed.Count);
        Assert.Equal(2, byUid["A"].Order);
        Assert.Equal(1, byUid["B"].Order);
    }

    [Fact]
    public void Reorder_Up_SwapsOrderWithPreviousVisibleNeighbor()
    {
        var group = new List<TerminalTask>
        {
            NewTask("A", date: DateA, order: 1),
            NewTask("B", date: DateA, order: 2),
        };

        var changed = TaskMutationService.Reorder(group, "B", Visible("A", "B"), ReorderDirection.Up);

        var byUid = changed.ToDictionary(task => task.Uid);
        Assert.Equal(1, byUid["B"].Order);
        Assert.Equal(2, byUid["A"].Order);
    }

    [Fact]
    public void Reorder_SkipsHiddenNeighbor_AndPreservesItsOrder()
    {
        var group = new List<TerminalTask>
        {
            NewTask("V1", date: DateA, order: 1),
            NewTask("H", date: DateA, order: 2, completed: true),
            NewTask("V2", date: DateA, order: 3),
        };

        var changed = TaskMutationService.Reorder(group, "V1", Visible("V1", "V2"), ReorderDirection.Down);

        // V1 swaps past the hidden task with the next visible task V2; the hidden task is untouched.
        var changedByUid = changed.ToDictionary(task => task.Uid);
        Assert.Equal(2, changed.Count);
        Assert.Equal(3, changedByUid["V1"].Order);
        Assert.Equal(1, changedByUid["V2"].Order);
        Assert.DoesNotContain(changed, task => task.Uid == "H");

        // After renumbering, the hidden task keeps its middle order and is not synced.
        var result = TaskOrderService.GetTasksToSync(group, changed);
        var resultByUid = result.ToDictionary(task => task.Uid);
        Assert.DoesNotContain(result, task => task.Uid == "H");
        Assert.Equal(3, resultByUid["V1"].Order);
        Assert.Equal(1, resultByUid["V2"].Order);
    }

    [Fact]
    public void Reorder_AtBoundary_ReturnsEmpty()
    {
        var group = new List<TerminalTask>
        {
            NewTask("A", date: DateA, order: 1),
            NewTask("B", date: DateA, order: 2),
        };

        var changed = TaskMutationService.Reorder(group, "A", Visible("A", "B"), ReorderDirection.Up);

        Assert.Empty(changed);
    }

    [Fact]
    public void Reorder_FocusNotInGroup_ReturnsEmpty()
    {
        var group = new List<TerminalTask> { NewTask("A", date: DateA, order: 1) };

        var changed = TaskMutationService.Reorder(group, "missing", Visible("A"), ReorderDirection.Down);

        Assert.Empty(changed);
    }

    // --- Field mutation commands ---
    [Fact]
    public void Create_UsesParsedFields_AndAppendsDatedTaskToEnd()
    {
        var parsed = new ParsedTaskText(DateA, 90, "Buy milk", TaskTextType.Routine, true);

        var task = TaskMutationService.Create(parsed, fallbackDate: DateB, uid: "new");

        Assert.Equal("new", task.Uid);
        Assert.Equal("Buy milk", task.Title);
        Assert.Equal(DateA, task.Date);
        Assert.Equal(90, task.Time);
        Assert.Equal(TerminalTaskType.Routine, task.Type);
        Assert.True(task.IsProbable);
        Assert.False(task.Completed);
        Assert.False(task.Deleted);
        Assert.Equal(0, task.Version);
        Assert.Equal(TaskMutationService.AppendOrder, task.Order);
    }

    [Fact]
    public void Create_WithoutParsedDate_UsesFallbackDate()
    {
        var parsed = new ParsedTaskText(null, null, "No date", TaskTextType.Simple, false);

        var task = TaskMutationService.Create(parsed, fallbackDate: DateB, uid: "x");

        Assert.Equal(DateB, task.Date);
    }

    [Fact]
    public void Create_WithoutParsedDateOrFallback_IsNoDate()
    {
        var parsed = new ParsedTaskText(null, null, "No date", TaskTextType.Simple, false);

        var task = TaskMutationService.Create(parsed, fallbackDate: null, uid: "x");

        Assert.Null(task.Date);
        Assert.Equal(TaskMutationService.PrependOrder, task.Order);
    }

    [Fact]
    public void Edit_MergesEditableFields_KeepsIdentityAndState()
    {
        var original = NewTask(
            "u",
            date: DateA,
            order: 5,
            version: 3,
            title: "Old",
            completed: true,
            time: 60,
            type: TerminalTaskType.Simple);
        var parsed = new ParsedTaskText(DateB, 120, "New", TaskTextType.Weekly, true);

        var edited = TaskMutationService.Edit(original, parsed);

        Assert.Equal("u", edited.Uid);
        Assert.Equal(5, edited.Order);
        Assert.Equal(3, edited.Version);
        Assert.True(edited.Completed);
        Assert.Equal("New", edited.Title);
        Assert.Equal(DateB, edited.Date);
        Assert.Equal(120, edited.Time);
        Assert.Equal(TerminalTaskType.Weekly, edited.Type);
        Assert.True(edited.IsProbable);
    }

    [Fact]
    public void Edit_WithoutParsedDate_ClearsDate()
    {
        var original = NewTask("u", date: DateA, title: "x");
        var parsed = new ParsedTaskText(null, null, "y", TaskTextType.Simple, false);

        var edited = TaskMutationService.Edit(original, parsed);

        Assert.Null(edited.Date);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void ToggleCompleted_FlipsCompleted(bool initial, bool expected)
    {
        var result = TaskMutationService.ToggleCompleted(NewTask("u", completed: initial));

        Assert.Equal(expected, result.Completed);
    }

    [Fact]
    public void Delete_SetsDeleted()
    {
        var result = TaskMutationService.Delete(NewTask("u"));

        Assert.True(result.Deleted);
    }

    [Fact]
    public void Move_SetsDate()
    {
        var result = TaskMutationService.Move(NewTask("u", date: DateA), DateB);

        Assert.Equal(DateB, result.Date);
    }

    [Fact]
    public void Move_ToNoDate_ClearsDate()
    {
        var result = TaskMutationService.Move(NewTask("u", date: DateA), null);

        Assert.Null(result.Date);
    }

    [Fact]
    public void MoveByDays_ShiftsDate()
    {
        Assert.Equal(DateA.AddDays(1), TaskMutationService.MoveByDays(NewTask("u", date: DateA), 1)!.Date);
        Assert.Equal(DateA.AddDays(-2), TaskMutationService.MoveByDays(NewTask("u", date: DateA), -2)!.Date);
    }

    [Fact]
    public void MoveByDays_NoDate_ReturnsNull()
    {
        Assert.Null(TaskMutationService.MoveByDays(NewTask("u", date: null), 1));
    }

    private static HashSet<string> Visible(params string[] uids)
    {
        return new HashSet<string>(uids);
    }

    private static TerminalTask NewTask(
        string uid,
        DateOnly? date = null,
        int order = 0,
        int version = 0,
        string title = "",
        bool completed = false,
        bool deleted = false,
        int? time = null,
        TerminalTaskType type = TerminalTaskType.Simple,
        bool isProbable = false)
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = title,
            Date = date,
            Time = time,
            Order = order,
            Completed = completed,
            Deleted = deleted,
            Type = type,
            IsProbable = isProbable,
            Version = version,
        };
    }
}
