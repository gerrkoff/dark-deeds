using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Editing;
using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Tasks;

namespace DD.TerminalClient.Domain.Application;

// The pure application state machine: it maps a key press (in any lifecycle phase), a resize, and the
// derived-view recompute into a new ApplicationState plus effects, using only the pure Domain services
// (projection, navigation, focus, mutations, ordering) and the shared parser/formatter. It owns no I/O,
// clock, or save queues, so the whole keymap and every lifecycle key path is deterministically testable.
// New task identities are drawn from the injected uid factory, keeping identity generation with the
// caller rather than embedding a clock or Guid dependency in the Domain.
public sealed class ApplicationReducer(
    TerminalInputReducer inputReducer,
    OverviewProjectionService projection,
    ITaskTextParser parser,
    TaskTextFormatter formatter,
    Func<string> uidFactory)
{
    private static readonly VisualTaskAddress OriginAddress = new()
    {
        Section = OverviewSection.NoDate,
        Row = 0,
        Column = 0,
        TaskIndex = 0,
    };

    // Rebuilds the Overview projection from the cache and toggles, then reconciles focus onto a still
    // visible task. Run after every cache or toggle change so navigation and rendering see one truth.
    public ApplicationState Recompute(ApplicationState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var projected = projection.Project(state.Cache, state.ShowCompleted, state.RoutineShownDates);
        var focus = TaskFocusService.Reconcile(projected, state.Focus);
        return state with { Projection = projected, Focus = focus };
    }

    public ApplicationTransition HandleKey(ApplicationState state, ConsoleKeyInfo key)
    {
        ArgumentNullException.ThrowIfNull(state);

        return state.Phase switch
        {
            ApplicationPhase.Login => HandleLoginKey(state, key),
            ApplicationPhase.ConfirmDataReset => HandleConfirmKey(state, key),
            ApplicationPhase.Ready => HandleReadyKey(state, key),
            _ => ApplicationTransition.Of(state),
        };
    }

    // Records the new terminal size and, in Ready phase, gates input into resize-required mode below the
    // supported minimum (and back to Normal when the window grows), while synchronization keeps running.
    public static ApplicationState HandleResize(ApplicationState state, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(state);

        var next = state with { Width = width, Height = height };
        if (state.Phase != ApplicationPhase.Ready)
        {
            return next;
        }

        var required = ViewportState.IsResizeRequired(width, height);
        if (required && next.Input.Mode == TerminalUiMode.Normal)
        {
            return next with { Input = next.Input with { Mode = TerminalUiMode.ResizeRequired } };
        }

        if (!required && next.Input.Mode == TerminalUiMode.ResizeRequired)
        {
            return next with { Input = TerminalInputState.Normal };
        }

        return next;
    }

    private ApplicationTransition HandleLoginKey(ApplicationState state, ConsoleKeyInfo key)
    {
        var result = inputReducer.Reduce(state.Input, key, TerminalInputContext.None);

        if (result.Command == TerminalCommand.SubmitLogin)
        {
            var text = result.CommittedText ?? string.Empty;
            if (state.LoginStep == LoginStep.Username)
            {
                return ApplicationTransition.Of(state with
                {
                    PendingUsername = text,
                    LoginStep = LoginStep.Password,
                    Input = TerminalInputState.BeginLogin(),
                    StatusMessage = null,
                });
            }

            return ApplicationTransition.Of(
                state with { Input = TerminalInputState.BeginLogin(), StatusMessage = "Signing in..." },
                ApplicationEffect.SignIn(state.PendingUsername ?? string.Empty, text));
        }

        // Any other key only edits the (masked) buffer; keep login mode even if Escape reset it to Normal.
        var nextInput = result.State.Mode == TerminalUiMode.MaskedLogin
            ? result.State
            : TerminalInputState.BeginLogin();
        return ApplicationTransition.Of(state with { Input = nextInput });
    }

    private static ApplicationTransition HandleConfirmKey(ApplicationState state, ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            return ApplicationTransition.Of(state with { Quit = true });
        }

        return key.KeyChar switch
        {
            'y' or 'Y' => ApplicationTransition.Of(state, ApplicationEffect.AcceptDataReset),
            'n' or 'N' => ApplicationTransition.Of(state with { Quit = true }),
            _ => ApplicationTransition.Of(state),
        };
    }

    private ApplicationTransition HandleReadyKey(ApplicationState state, ConsoleKeyInfo key)
    {
        var context = BuildContext(state);
        var result = inputReducer.Reduce(state.Input, key, context);
        var next = state with { Input = result.State, StatusMessage = result.StatusMessage };

        return result.Command == TerminalCommand.None
            ? ApplicationTransition.Of(next)
            : ExecuteCommand(next, result.Command, result.CommittedText);
    }

    private ApplicationTransition ExecuteCommand(ApplicationState state, TerminalCommand command, string? text)
    {
        return command switch
        {
            TerminalCommand.None => ApplicationTransition.Of(state),
            TerminalCommand.NavigateUp => Navigate(state, NavigationDirection.Up),
            TerminalCommand.NavigateDown => Navigate(state, NavigationDirection.Down),
            TerminalCommand.NavigateLeft => Navigate(state, NavigationDirection.Left),
            TerminalCommand.NavigateRight => Navigate(state, NavigationDirection.Right),
            TerminalCommand.ReorderUp => Reorder(state, ReorderDirection.Up),
            TerminalCommand.ReorderDown => Reorder(state, ReorderDirection.Down),
            TerminalCommand.MoveDayBackward => MoveByDay(state, -1),
            TerminalCommand.MoveDayForward => MoveByDay(state, 1),
            TerminalCommand.ToggleComplete => MutateFocused(state, TaskMutationService.ToggleCompleted),
            TerminalCommand.ConfirmDelete => MutateFocused(state, TaskMutationService.Delete),
            TerminalCommand.ToggleCompletedVisibility => ToggleCompletedVisibility(state),
            TerminalCommand.ToggleRoutine => ToggleRoutine(state),
            TerminalCommand.ForceReconnect => ApplicationTransition.Of(
                state with { IsBuffering = true, StatusMessage = "Reconnecting..." },
                ApplicationEffect.Reconnect),
            TerminalCommand.Quit => ApplicationTransition.Of(state with { Quit = true }),
            TerminalCommand.SubmitAddWithFocusDate => Create(state, text, FindFocused(state)?.Date),
            TerminalCommand.SubmitAddNoDate => Create(state, text, fallback: null),
            TerminalCommand.SubmitEdit => EditFocused(state, text),
            TerminalCommand.SubmitMove => MoveFocused(state, text),
            TerminalCommand.SubmitLogin => ApplicationTransition.Of(state),
            _ => ApplicationTransition.Of(state),
        };
    }

    private static ApplicationTransition Navigate(ApplicationState state, NavigationDirection direction)
    {
        if (state.Focus is null)
        {
            return ApplicationTransition.Of(state);
        }

        var moved = TaskNavigationService.Move(state.Projection, state.Focus, direction);
        return ApplicationTransition.Of(state with { Focus = moved, StatusMessage = null });
    }

    private ApplicationTransition MutateFocused(ApplicationState state, Func<TerminalTask, TerminalTask> mutate)
    {
        var focused = FindFocused(state);
        return focused is null
            ? Decline(state, "No task selected.")
            : CommitChanges(state, [mutate(focused)], focusUid: null);
    }

    private ApplicationTransition MoveByDay(ApplicationState state, int days)
    {
        var focused = FindFocused(state);
        if (focused is null)
        {
            return Decline(state, "No task selected.");
        }

        var moved = TaskMutationService.MoveByDays(focused, days);
        return moved is null
            ? Decline(state, "Set a date before moving a No Date task by day.")
            : CommitChanges(state, [moved], focusUid: null);
    }

    private ApplicationTransition Reorder(ApplicationState state, ReorderDirection direction)
    {
        var focused = FindFocused(state);
        if (focused is null)
        {
            return Decline(state, "No task selected.");
        }

        var group = state.Cache.Where(task => !task.Deleted && task.Date == focused.Date).ToList();
        var visible = VisibleUidsOnDate(state.Projection, focused.Date);
        var swapped = TaskMutationService.Reorder(group, focused.Uid, visible, direction);
        return swapped.Count == 0
            ? Decline(state, "No task to swap with.")
            : CommitChanges(state, swapped, focusUid: null);
    }

    private ApplicationTransition Create(ApplicationState state, string? text, DateOnly? fallback)
    {
        IReadOnlyList<ParsedTaskText> parsed;
        try
        {
            parsed = parser.Parse(text ?? string.Empty);
        }
        catch (TaskTextParseException exception)
        {
            return Decline(state, exception.Message);
        }

        if (parsed.Count == 0)
        {
            return ApplicationTransition.Of(state);
        }

        var created = parsed.Select(item => TaskMutationService.Create(item, fallback, uidFactory())).ToList();
        return CommitChanges(state, created, focusUid: created[0].Uid);
    }

    private ApplicationTransition EditFocused(ApplicationState state, string? text)
    {
        var focused = FindFocused(state);
        if (focused is null)
        {
            return Decline(state, "No task selected.");
        }

        IReadOnlyList<ParsedTaskText> parsed;
        try
        {
            parsed = parser.Parse(text ?? string.Empty);
        }
        catch (TaskTextParseException exception)
        {
            return Decline(state, exception.Message);
        }

        return parsed.Count == 0
            ? ApplicationTransition.Of(state)
            : CommitChanges(state, [TaskMutationService.Edit(focused, parsed[0])], focusUid: null);
    }

    private ApplicationTransition MoveFocused(ApplicationState state, string? text)
    {
        var focused = FindFocused(state);
        if (focused is null)
        {
            return Decline(state, "No task selected.");
        }

        var (ok, date, error) = ParseMoveDate(text);
        return ok
            ? CommitChanges(state, [TaskMutationService.Move(focused, date)], focusUid: null)
            : Decline(state, error ?? "Enter a valid date.");
    }

    private (bool Ok, DateOnly? Date, string? Error) ParseMoveDate(string? text)
    {
        var trimmed = (text ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return (true, null, null);
        }

        try
        {
            // Reuse the shared date grammar by appending a placeholder title, then keep only the date.
            var parsed = parser.Parse(trimmed + " x");
            return (true, parsed.Count > 0 ? parsed[0].Date : null, null);
        }
        catch (TaskTextParseException exception)
        {
            return (false, null, exception.Message);
        }
    }

    private ApplicationTransition ToggleCompletedVisibility(ApplicationState state)
    {
        return ApplicationTransition.Of(Recompute(state with { ShowCompleted = !state.ShowCompleted }));
    }

    private ApplicationTransition ToggleRoutine(ApplicationState state)
    {
        var focused = FindFocused(state);
        if (focused?.Date is not { } date)
        {
            return Decline(state, "Routine visibility applies to a dated task.");
        }

        var set = new HashSet<DateOnly>(state.RoutineShownDates);
        if (!set.Remove(date))
        {
            set.Add(date);
        }

        return ApplicationTransition.Of(Recompute(state with { RoutineShownDates = set }));
    }

    private ApplicationTransition CommitChanges(
        ApplicationState state, IReadOnlyList<TerminalTask> changed, string? focusUid)
    {
        var sync = TaskOrderService.GetTasksToSync(state.Cache, changed);
        var next = state with { Cache = MergeCache(state.Cache, sync) };
        if (focusUid is not null)
        {
            next = next with { Focus = new TaskFocus { Uid = focusUid, Address = OriginAddress } };
        }

        next = Recompute(next);
        return sync.Count == 0
            ? ApplicationTransition.Of(next)
            : ApplicationTransition.Of(next, ApplicationEffect.EnqueueTaskChanges(sync));
    }

    // Applies the renumbered sync batch to the cache: existing tasks are replaced in place (preserving
    // order) and genuinely new tasks are appended.
    private static IReadOnlyList<TerminalTask> MergeCache(
        IReadOnlyList<TerminalTask> cache, IReadOnlyList<TerminalTask> sync)
    {
        if (sync.Count == 0)
        {
            return cache;
        }

        var byUid = sync.ToDictionary(task => task.Uid, StringComparer.Ordinal);
        var merged = new List<TerminalTask>(cache.Count + sync.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var task in cache)
        {
            if (byUid.TryGetValue(task.Uid, out var updated))
            {
                merged.Add(updated);
                seen.Add(task.Uid);
            }
            else
            {
                merged.Add(task);
            }
        }

        foreach (var task in sync)
        {
            if (seen.Add(task.Uid))
            {
                merged.Add(task);
            }
        }

        return merged;
    }

    private static HashSet<string> VisibleUidsOnDate(OverviewProjection projection, DateOnly? date)
    {
        var uids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var cell in AllCells(projection))
        {
            foreach (var task in cell.Tasks)
            {
                if (task.Task.Date == date)
                {
                    uids.Add(task.Task.Uid);
                }
            }
        }

        return uids;
    }

    private static IEnumerable<OverviewDay> AllCells(OverviewProjection projection)
    {
        yield return projection.NoDate;
        foreach (var cell in projection.Overdue)
        {
            yield return cell;
        }

        foreach (var cell in projection.Current)
        {
            yield return cell;
        }

        foreach (var cell in projection.Future)
        {
            yield return cell;
        }
    }

    private TerminalInputContext BuildContext(ApplicationState state)
    {
        var focused = FindFocused(state);
        if (focused is null)
        {
            return TerminalInputContext.None;
        }

        return new TerminalInputContext
        {
            HasFocus = true,
            FocusHasDate = focused.Date is not null,
            FocusDate = focused.Date,
            EditText = formatter.Format(focused),
            MoveText = string.Empty,
        };
    }

    private static TerminalTask? FindFocused(ApplicationState state)
    {
        var uid = state.Focus?.Uid;
        return uid is null
            ? null
            : state.Cache.FirstOrDefault(task => string.Equals(task.Uid, uid, StringComparison.Ordinal));
    }

    private static ApplicationTransition Decline(ApplicationState state, string message)
    {
        return ApplicationTransition.Of(state with { StatusMessage = message });
    }
}
