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
            // Restore the interaction suspended when the terminal dropped below the minimum (an open
            // editor or delete confirmation and its draft), or fall back to Normal when nothing
            // interactive was suspended (a plain resize-required screen).
            return next with { Input = next.SuspendedInput ?? TerminalInputState.Normal, SuspendedInput = null };
        }

        return next;
    }

    private ApplicationTransition HandleLoginKey(ApplicationState state, ConsoleKeyInfo key)
    {
        // A sign-in is in flight: ignore every key so the user cannot submit a second, concurrent attempt
        // whose late completion could overwrite the result of the first (for example returning an
        // already-authenticated session to login).
        if (state.SigningIn)
        {
            return ApplicationTransition.Of(state);
        }

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
                state with
                {
                    Input = TerminalInputState.BeginLogin(),
                    SigningIn = true,
                    StatusMessage = "Signing in...",
                },
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

        // Gate before reducing as well as after: a terminal that first reaches Ready already below the
        // minimum lands in Normal mode with no resize event to gate it, so reducing the raw Normal state
        // would run a hidden command on the first key press. Gating the input first routes that key
        // through resize-required (which accepts only ? and q) instead.
        var result = inputReducer.Reduce(GateInputByResize(state, state.Input), key, context);

        // Closing Help resumes a suspended interaction rather than dropping to Normal: when the terminal
        // grew back to a usable size while Help was open, the grow-back resize could not consume the
        // snapshot (the mode was Help, not ResizeRequired), so closing Help is the last chance to restore
        // the editor/confirmation and its draft instead of discarding it.
        var reduced = ResumeSuspendedInputOnHelpClose(state, result.State);
        var next = state with
        {
            Input = GateInputByResize(state, reduced),
            StatusMessage = result.StatusMessage,
            SuspendedInput = SuspendInputForResize(state),
        };

        // The target and the inherited add-date captured when the modal opened (the pre-reduce input state
        // still carries them; the commit resets the input to Normal). A commit uses these, not the live
        // focus, so a realtime update that moved or re-dated the focused task while the modal was open
        // cannot redirect the edit/move/delete to another task or create the new task on another day.
        var targetUid = state.Input.TargetUid;
        var fallbackDate = state.Input.FallbackDate;

        return result.Command == TerminalCommand.None
            ? ApplicationTransition.Of(next)
            : ExecuteCommand(next, result.Command, result.CommittedText, targetUid, fallbackDate);
    }

    // When Help closes over a still-pending suspended interaction, resume that interaction instead of the
    // Normal state the input reducer returns. This is the grow-back path HandleResize cannot cover: growing
    // back to a usable size while Help is open leaves the snapshot in place (the mode is Help, not
    // ResizeRequired, so the resize handler never consumes it), and at a usable size the next
    // SuspendInputForResize would clear it - so restoring it as Help closes is the only remaining chance to
    // avoid discarding the editor/confirmation and its draft. Below the minimum the resumed input is simply
    // re-gated to resize-required, so this leaves the still-too-small case unchanged.
    private static TerminalInputState ResumeSuspendedInputOnHelpClose(
        ApplicationState state, TerminalInputState reduced)
    {
        return state.Input.Mode == TerminalUiMode.Help
               && reduced.Mode != TerminalUiMode.Help
               && state.SuspendedInput is { } suspended
            ? suspended
            : reduced;
    }

    // The interaction to restore when the terminal grows back to a usable size. Below the minimum it
    // snapshots the live editor/confirmation (and its draft) the first time a key is gated, so gating that
    // key into resize-required (and the eventual grow-back) does not silently discard it; once captured it
    // is kept until grow-back consumes it, so an intervening Help overlay or ignored key cannot lose it.
    // Nothing is suspended at a usable size, and Help/ResizeRequired are never snapshotted - Help rides on
    // top of the already-snapshotted suspended state and ResizeRequired is the gated state itself.
    private static TerminalInputState? SuspendInputForResize(ApplicationState state)
    {
        if (!ViewportState.IsResizeRequired(state.Width, state.Height))
        {
            return null;
        }

        return state.SuspendedInput
               ?? (state.Input.Mode is TerminalUiMode.ResizeRequired or TerminalUiMode.Help
                   ? null
                   : state.Input);
    }

    // Forces resize-required whenever an input state would otherwise sit in an interactive mode while the
    // terminal is below the supported minimum, keeping every hidden command unreachable. It gates every
    // mode except ResizeRequired itself and Help: Normal (its keymap commands), Editor and
    // DeleteConfirmation (whose Enter/'y' would otherwise commit or delete against a modal the size-gated
    // renderer has hidden) all collapse to resize-required, while Help stays reachable because the resize
    // screen advertises '? help' and the renderer shows Help even below the minimum. HandleReadyKey applies
    // it both before reducing a key (so a terminal that first reached Ready below the minimum, or shrank
    // while a modal was open, cannot run a hidden command on the next key press) and after (so closing Help
    // returns to resize-required). This is necessary because the size timer only re-emits a resize event
    // when the dimensions change, so a still-too-small terminal would otherwise never re-enter
    // resize-required.
    private static TerminalInputState GateInputByResize(ApplicationState state, TerminalInputState input)
    {
        return input.Mode is not (TerminalUiMode.ResizeRequired or TerminalUiMode.Help)
               && ViewportState.IsResizeRequired(state.Width, state.Height)
            ? input with { Mode = TerminalUiMode.ResizeRequired }
            : input;
    }

    private ApplicationTransition ExecuteCommand(
        ApplicationState state, TerminalCommand command, string? text, string? targetUid, DateOnly? fallbackDate)
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
            TerminalCommand.ConfirmDelete => DeleteTarget(state, targetUid),
            TerminalCommand.ToggleCompletedVisibility => ToggleCompletedVisibility(state),
            TerminalCommand.ToggleRoutine => ToggleRoutine(state),
            TerminalCommand.ForceReconnect => ApplicationTransition.Of(
                state with { IsBuffering = true, StatusMessage = "Reconnecting..." },
                ApplicationEffect.Reconnect),
            TerminalCommand.Quit => ApplicationTransition.Of(state with { Quit = true }),
            TerminalCommand.SubmitAddWithFocusDate => Create(state, text, fallbackDate),
            TerminalCommand.SubmitAddNoDate => Create(state, text, fallback: null),
            TerminalCommand.SubmitEdit => EditTarget(state, text, targetUid),
            TerminalCommand.SubmitMove => MoveTarget(state, text, targetUid),
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

    private ApplicationTransition EditTarget(ApplicationState state, string? text, string? targetUid)
    {
        var target = FindByUid(state, targetUid);
        if (target is null)
        {
            return Decline(state, "The task is no longer available.");
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

        if (parsed.Count == 0)
        {
            return ApplicationTransition.Of(state);
        }

        // An edit renames a single task; a range (many parsed results) has no single-task meaning, so it is
        // rejected rather than silently applying only the first day's parse.
        if (parsed.Count > 1)
        {
            return Decline(state, "An edit must be a single task, not a date range.");
        }

        return CommitChanges(state, [TaskMutationService.Edit(target, parsed[0])], focusUid: null);
    }

    private ApplicationTransition MoveTarget(ApplicationState state, string? text, string? targetUid)
    {
        var target = FindByUid(state, targetUid);
        if (target is null)
        {
            return Decline(state, "The task is no longer available.");
        }

        var (ok, date, error) = ParseMoveDate(text);
        return ok
            ? CommitChanges(state, [TaskMutationService.Move(target, date)], focusUid: null)
            : Decline(state, error ?? "Enter a valid date.");
    }

    private ApplicationTransition DeleteTarget(ApplicationState state, string? targetUid)
    {
        var target = FindByUid(state, targetUid);
        return target is null
            ? Decline(state, "The task is no longer available.")
            : CommitChanges(state, [TaskMutationService.Delete(target)], focusUid: null);
    }

    private (bool Ok, DateOnly? Date, string? Error) ParseMoveDate(string? text)
    {
        var trimmed = (text ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            // An empty move is the explicit "move to No Date" action.
            return (true, null, null);
        }

        try
        {
            // Reuse the shared date grammar by appending a placeholder title, then keep only the date. A
            // move targets exactly one date, so a range (many parsed results) or free text that carries no
            // date is rejected rather than silently collapsing to the first day or clearing the date.
            var parsed = parser.Parse(trimmed + " x");
            if (parsed.Count != 1)
            {
                return (false, null, "Enter a single date, or leave it empty for No Date.");
            }

            if (parsed[0].Date is not { } date)
            {
                return (false, null, "Enter a valid date, or leave it empty for No Date.");
            }

            return (true, date, null);
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
            FocusUid = focused.Uid,
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

    // Resolves a modal's captured target task by Uid from the cache. Returns null when the task no longer
    // exists (a soft-delete keeps it in the cache, so this only misses a task the server has since removed),
    // so the commit declines instead of touching the wrong task or resurrecting a removed one.
    private static TerminalTask? FindByUid(ApplicationState state, string? uid)
    {
        return uid is null
            ? null
            : state.Cache.FirstOrDefault(task => string.Equals(task.Uid, uid, StringComparison.Ordinal));
    }

    private static ApplicationTransition Decline(ApplicationState state, string message)
    {
        return ApplicationTransition.Of(state with { StatusMessage = message });
    }
}
