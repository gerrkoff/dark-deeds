using System.Threading.Channels;
using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Application;
using DD.TerminalClient.Domain.Authentication;
using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Realtime;
using DD.TerminalClient.Domain.State;
using DD.TerminalClient.Domain.Synchronization;

namespace DD.TerminalClient;

// The application loop: the single reader of the event channel and the single writer of application and
// UI state. Every producer - the keyboard reader, the SignalR hub sink, the REST/renew/save completions,
// and the retry/renewal/resize timers - only enqueues an ApplicationEvent; this loop drains a batch,
// applies pure transitions through ApplicationReducer, runs the resulting effects (save coordinator, hub,
// auth, persistence), and paints one coalesced frame. It owns the crash-safe startup/reconnect/401
// sequence and, in its finally, persists state (leaving unsaved edits in the outbox), stops the hub and
// timers, and restores the terminal screen.
internal sealed class TerminalApplication
{
    private static readonly TimeSpan ReloadRetryDelay = TimeSpan.FromSeconds(5);

    private readonly Channel<ApplicationEvent> _channel =
        Channel.CreateUnbounded<ApplicationEvent>(new UnboundedChannelOptions { SingleReader = true });

    private readonly TaskSyncCoordinator _sync = new();
    private readonly TaskReconciler _reconciler;
    private readonly TerminalDependencies _deps;
    private readonly ITaskHubClient _hub;

    private AuthSession? _session;
    private string? _dataOwner;
    private int _priorOutboxCount;
    private IReadOnlyList<TerminalTask>? _preservedOutbox;
    private bool _firstSnapshotDone;
    private int _snapshotGeneration;
    private bool _renewing;
    private bool _stateLoadFailed;
    private CancellationToken _appToken;
    private CancellationTokenSource? _lifetimeCts;
    private CancellationTokenSource? _sessionCts;

    public TerminalApplication(TerminalDependencies dependencies)
    {
        ArgumentNullException.ThrowIfNull(dependencies);
        _deps = dependencies;
        _reconciler = new TaskReconciler(_sync);
        _hub = dependencies.HubFactory(hubEvent => Enqueue(ApplicationEvent.HubEvent(hubEvent)));

        var (width, height) = dependencies.ReadDimensions();
        State = new ApplicationState { Width = width, Height = height };
    }

    // A blocking, actionable message when startup or the loop failed fatally; the composition root prints
    // it to stderr after the screen is restored. Null on a clean exit.
    public string? FatalMessage { get; private set; }

    // The current immutable state snapshot. Exposed for tests; a reference read of an immutable value.
    internal ApplicationState State { get; private set; }

    // The cancellation scope for the current signed-in session's REST activity (snapshot, save, renew).
    // A fresh scope is created when online startup begins and is cancelled on a 401 so an in-flight
    // request cannot complete and resurrect the session or mutate state after we return to login.
    private CancellationToken SessionToken => _sessionCts?.Token ?? _appToken;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _lifetimeCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _appToken = _lifetimeCts.Token;

        try
        {
            _deps.Renderer.Begin();
            Initialize();

            if (!State.Quit)
            {
                if (State.Phase == ApplicationPhase.Ready)
                {
                    BeginOnlineStartup();
                }

                Render();
                StartProducers();
                await EventLoopAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Cooperative shutdown requested through the cancellation token.
        }
#pragma warning disable CA1031
        catch (Exception exception)
        {
            FatalMessage = $"The terminal client stopped after an unexpected error ({exception.GetType().Name}).";
        }
#pragma warning restore CA1031
        finally
        {
            await ShutdownAsync();
        }
    }

    internal bool Enqueue(ApplicationEvent applicationEvent)
    {
        return _channel.Writer.TryWrite(applicationEvent);
    }

    private async Task EventLoopAsync()
    {
        while (!State.Quit)
        {
            if (!await _channel.Reader.WaitToReadAsync(_appToken))
            {
                break;
            }

            while (_channel.Reader.TryRead(out var applicationEvent))
            {
                Process(applicationEvent);
                if (State.Quit)
                {
                    break;
                }
            }

            Render();
        }
    }

    private void Process(ApplicationEvent applicationEvent)
    {
        switch (applicationEvent.Kind)
        {
            case ApplicationEventKind.Key:
                HandleKey(applicationEvent.Key);
                break;
            case ApplicationEventKind.Resize:
                State = ApplicationReducer.HandleResize(State, applicationEvent.Width, applicationEvent.Height);
                break;
            case ApplicationEventKind.Hub:
                HandleHub(applicationEvent.Hub!);
                break;
            case ApplicationEventKind.SnapshotLoaded:
                HandleSnapshotLoaded(applicationEvent.Tasks, applicationEvent.Generation);
                break;
            case ApplicationEventKind.SnapshotFailed:
                HandleSnapshotFailed(applicationEvent.Unauthorized, applicationEvent.Generation);
                break;
            case ApplicationEventKind.SignInCompleted:
                HandleSignInCompleted(applicationEvent.SignIn!);
                break;
            case ApplicationEventKind.SignInFaulted:
                ReturnToLogin(applicationEvent.Message);
                break;
            case ApplicationEventKind.SaveCompleted:
                HandleSaveCompleted(applicationEvent.Tasks);
                break;
            case ApplicationEventKind.SaveFaulted:
                HandleSaveFaulted(applicationEvent.Unauthorized);
                break;
            case ApplicationEventKind.RetryTick:
                RunSyncEffects(_sync.OnRetryElapsed());
                break;
            case ApplicationEventKind.ReloadSnapshotTick:
                if (_session is not null && State.IsBuffering)
                {
                    StartSnapshotLoad();
                }

                break;
            case ApplicationEventKind.RenewTick:
                MaybeRenew();
                break;
            case ApplicationEventKind.RenewCompleted:
                HandleRenewCompleted(applicationEvent.Session!);
                break;
            case ApplicationEventKind.RenewFaulted:
                _renewing = false;
                if (applicationEvent.Unauthorized)
                {
                    Handle401();
                }

                break;
            default:
                break;
        }
    }

    private void HandleKey(ConsoleKeyInfo key)
    {
        var transition = _deps.Reducer.HandleKey(State, key);
        State = transition.State;
        RunEffects(transition.Effects);
    }

    private void RunEffects(IReadOnlyList<ApplicationEffect> effects)
    {
        foreach (var effect in effects)
        {
            switch (effect.Kind)
            {
                case ApplicationEffectKind.EnqueueTaskChanges:
                    RunSyncEffects(_sync.EnqueueLocalChanges(effect.Tasks));
                    break;
                case ApplicationEffectKind.SignIn:
                    StartSignIn(effect.Username, effect.Password);
                    break;
                case ApplicationEffectKind.Reconnect:
                    StartReconnect();
                    break;
                case ApplicationEffectKind.AcceptDataReset:
                    AcceptDataReset();
                    break;
                default:
                    break;
            }
        }
    }

    private void RunSyncEffects(IReadOnlyList<TaskSyncEffect> effects)
    {
        foreach (var effect in effects)
        {
            switch (effect.Kind)
            {
                case TaskSyncEffectKind.PersistOutbox:
                    PersistState();
                    break;
                case TaskSyncEffectKind.SaveBatch:
                    if (_firstSnapshotDone)
                    {
                        StartSaveBatch(effect.Tasks);
                    }

                    break;
                case TaskSyncEffectKind.ScheduleRetry:
                    StartDelayedTick(effect.RetryDelay, ApplicationEvent.RetryTick);
                    break;
                case TaskSyncEffectKind.ReportSyncStatus:
                    State = State with { IsSaving = effect.IsSaving };
                    break;
                case TaskSyncEffectKind.SaveFinished:
                    ApplySaveFinished(effect);
                    break;
                default:
                    break;
            }
        }
    }

    private void ApplySaveFinished(TaskSyncEffect effect)
    {
        if (effect.Saved.Count > 0)
        {
            State = State with { Cache = ApplyVersions(State.Cache, effect.Saved) };
        }

        if (effect.Conflicted.Count > 0)
        {
            State = State with { Notification = BuildConflictNotice(effect.Conflicted) };
        }

        if (effect.NotSaved > 0)
        {
            State = State with { IsOffline = true, StatusMessage = "Offline - the save will be retried." };
        }

        State = _deps.Reducer.Recompute(State);
    }

    private void HandleHub(TaskHubEvent hubEvent)
    {
        switch (hubEvent.Kind)
        {
            case TaskHubEventKind.Update:
                ApplyReconciliation(_reconciler.ProcessOnlineUpdate(hubEvent.Tasks));
                State = _deps.Reducer.Recompute(State with { IsOffline = false });
                PersistState();
                break;
            case TaskHubEventKind.Heartbeat:
                State = State with { IsOffline = false };
                break;
            case TaskHubEventKind.Reconnecting:
                State = State with { IsOffline = true, IsBuffering = true, StatusMessage = "Reconnecting..." };
                break;
            case TaskHubEventKind.Reconnected:
                State = State with { IsBuffering = true, IsOffline = false };
                StartSnapshotLoad();
                break;
            case TaskHubEventKind.Closed:
                break;
            case TaskHubEventKind.Unauthorized:
                Handle401();
                break;
        }
    }

    private void HandleSnapshotLoaded(IReadOnlyList<TerminalTask> tasks, int generation)
    {
        // Discard a superseded load: a newer snapshot request (a reconnect reload, a retry) has started
        // since this one, so re-reconciling its stale result could remove tasks the newer state already has.
        if (generation != _snapshotGeneration)
        {
            return;
        }

        ApplyReconciliation(_reconciler.ReconcileSnapshot(tasks));

        foreach (var buffered in _hub.DrainBufferedUpdates())
        {
            if (buffered.Kind == TaskHubEventKind.Update)
            {
                ApplyReconciliation(_reconciler.ProcessOnlineUpdate(buffered.Tasks));
            }
        }

        if (!_firstSnapshotDone)
        {
            _firstSnapshotDone = true;
            DrainRestoredOutbox();
        }

        State = _deps.Reducer.Recompute(State with { IsBuffering = false, IsOffline = false });
        PersistState();
    }

    // Sends the startup outbox only after the first snapshot has been reconciled: a snapshot that predates
    // a queued edit therefore cannot revert it, and any server-won conflict has already dropped its Uid.
    private void DrainRestoredOutbox()
    {
        List<TerminalTask> batch = [.. _sync.State.InFlight.Values];
        if (batch.Count > 0)
        {
            StartSaveBatch(batch);
        }
        else if (_sync.State.IsSaving)
        {
            // The whole restored batch was dropped by snapshot conflicts; settle the coordinator.
            RunSyncEffects(_sync.OnSaveSucceeded([]));
        }
    }

    private void HandleSnapshotFailed(bool unauthorized, int generation)
    {
        // A 401 invalidates the whole session regardless of which load observed it, so it is always
        // honoured; a plain offline failure from a superseded load is discarded so it neither overwrites a
        // newer load's status nor schedules a duplicate retry.
        if (unauthorized)
        {
            Handle401();
            return;
        }

        if (generation != _snapshotGeneration)
        {
            return;
        }

        State = State with { IsOffline = true, StatusMessage = "Offline - showing cached tasks." };
        StartDelayedTick(ReloadRetryDelay, ApplicationEvent.ReloadSnapshotTick);
    }

    private void HandleSignInCompleted(SignInOutcome outcome)
    {
        switch (outcome.Status)
        {
            case TerminalSignInStatus.Success when outcome.Session is not null:
                var session = outcome.Session;
                _session = session;
                _deps.SetToken(session.Token);
                ProceedAfterAuth(session.Username, startOnline: true);
                break;
            case TerminalSignInStatus.InvalidCredentials:
                ReturnToLogin("Invalid username or password.");
                break;
            default:
                ReturnToLogin("Sign-in failed. Please try again.");
                break;
        }
    }

    private void HandleSaveCompleted(IReadOnlyList<TerminalTask> saved)
    {
        State = State with { IsOffline = false };
        RunSyncEffects(_sync.OnSaveSucceeded(saved));
    }

    private void HandleSaveFaulted(bool unauthorized)
    {
        if (unauthorized)
        {
            Handle401();
            return;
        }

        State = State with { IsOffline = true };
        RunSyncEffects(_sync.OnSaveFailed());
    }

    private void HandleRenewCompleted(AuthSession session)
    {
        _renewing = false;
        _session = session;
        _deps.SetToken(session.Token);
        _deps.TokenStore.Save(session.Token);
    }

    private void MaybeRenew()
    {
        if (_session is null || _renewing)
        {
            return;
        }

        if (_session.ExpiresAt is { } expiry && expiry - _deps.Clock.GetUtcNow() < TimeSpan.FromDays(1))
        {
            _renewing = true;
            _ = RenewAsync();
        }
    }

    private void Initialize()
    {
        PersistedTerminalState? persisted;
        try
        {
            persisted = _deps.StateStore.Load();
        }
        catch (TerminalStateException exception)
        {
            // The existing file is malformed or newer-than-supported and must be retained, not silently
            // replaced. Flag the failure so shutdown persistence never overwrites it with default state.
            _stateLoadFailed = true;
            FatalMessage = exception.Message;
            State = State with { Quit = true };
            return;
        }

        _dataOwner = persisted?.DataOwner;
        _priorOutboxCount = persisted?.Outbox.Count ?? 0;

        // Preserve the durable outbox from the moment it is loaded until it is either replayed for the
        // same user (RestorePersistedOutbox) or explicitly discarded for a different user (AcceptDataReset).
        // Any path that reaches login or the data-owner reset prompt without first restoring it - an
        // absent/unusable/expired token, or a startup that lands straight on a different-user reset because
        // a usable token names another owner - would otherwise let a quit flush an empty outbox over the
        // queued edits still on disk.
        _preservedOutbox = persisted?.Outbox;

        State = State with
        {
            Cache = persisted?.CachedTasks ?? [],
            ShowCompleted = persisted?.ShowCompleted ?? false,
        };

        var token = _deps.TokenStore.Load();
        var session = token is null ? null : AuthSession.FromToken(token);

        if (session is null || !session.HasUsableShape || session.IsExpired(_deps.Clock.GetUtcNow()))
        {
            _session = null;
            _deps.SetToken(null);
            State = _deps.Reducer.Recompute(State with
            {
                Phase = ApplicationPhase.Login,
                LoginStep = LoginStep.Username,
                Input = TerminalInputState.BeginLogin(),
            });
            return;
        }

        _session = session;
        _deps.SetToken(session.Token);
        ProceedAfterAuth(session.Username, startOnline: false);
    }

    private void ProceedAfterAuth(string? user, bool startOnline)
    {
        var hasPriorData = State.Cache.Count > 0 || _priorOutboxCount > 0;
        if (_dataOwner is not null && user is not null
            && !string.Equals(_dataOwner, user, StringComparison.Ordinal) && hasPriorData)
        {
            State = State with
            {
                Phase = ApplicationPhase.ConfirmDataReset,
                ConfirmUser = user,
                ConfirmOwner = _dataOwner,
                SigningIn = false,
                StatusMessage = null,
            };
            return;
        }

        _dataOwner = user;
        if (startOnline && _session is not null)
        {
            // Persist the token only once the sign-in is committed to a session - past the different-user
            // reset prompt - so declining that prompt never leaves the new user's token stored over the
            // prior owner's local data (which would send the next launch straight back to the reset prompt
            // with no path to login). Startup (startOnline: false) already loaded its token from disk.
            _deps.TokenStore.Save(_session.Token);
        }

        RestorePersistedOutbox();
        State = _deps.Reducer.Recompute(State with
        {
            Phase = ApplicationPhase.Ready,
            SigningIn = false,
            Input = TerminalInputState.Normal,
            SuspendedInput = null,
            StatusMessage = null,
        });

        if (startOnline)
        {
            BeginOnlineStartup();
        }
    }

    private void AcceptDataReset()
    {
        _dataOwner = State.ConfirmUser;
        _preservedOutbox = null;
        _sync.Reset();
        if (_session is not null)
        {
            // The different-user sign-in deferred persisting its token until the reset was accepted; now
            // that the prior owner's local data is cleared, commit the new owner's token to disk.
            _deps.TokenStore.Save(_session.Token);
        }

        State = State with
        {
            Cache = [],
            Phase = ApplicationPhase.Ready,
            Input = TerminalInputState.Normal,
            SuspendedInput = null,
            ConfirmUser = null,
            ConfirmOwner = null,
            StatusMessage = null,
        };
        PersistState();
        State = _deps.Reducer.Recompute(State);
        BeginOnlineStartup();
    }

    private void ReturnToLogin(string message)
    {
        State = State with
        {
            Phase = ApplicationPhase.Login,
            LoginStep = LoginStep.Username,
            PendingUsername = null,
            SigningIn = false,
            Input = TerminalInputState.BeginLogin(),
            StatusMessage = message,
        };
    }

    private void Handle401()
    {
        // Stop this session's in-flight REST activity (snapshot, save, renew) so a late completion cannot
        // resurrect the session or mutate state after we have returned to the login screen.
        _sessionCts?.Cancel();

        // Supersede any snapshot load still in flight from the now-dead session: cancellation cannot
        // unwind a response that already returned before it propagated, so LoadSnapshotAsync can still
        // enqueue SnapshotLoaded tagged with the current generation. Bumping the generation here - the
        // same supersession the reconnect/retry reloads use - makes that stale load a no-op instead of
        // letting it reconcile expired-session state and, worse, set _firstSnapshotDone at the login
        // screen, which would make the next sign-in replay its outbox before the fresh snapshot reconciles.
        _snapshotGeneration++;

        // Preserve the durable outbox: Reset clears only the in-memory queues, so capture their contents
        // first and keep persisting them until the same user signs back in and replays them. Capture only
        // once per unauthorized episode with ??=: a second 401 for the same expired token (for example a
        // hub Unauthorized alongside a REST or renewal 401) must not recompute this from the already-Reset
        // coordinator and overwrite the real contents with an empty list that the next PersistState would
        // then flush to disk. RestorePersistedOutbox/AcceptDataReset clear it, so a later 401 recaptures.
        _preservedOutbox ??= _sync.BuildOutboxContents();
        _ = StopHubQuietlyAsync();
        _sync.Reset();
        _firstSnapshotDone = false;

        // Cancelling the session above also cancels any in-flight token renewal, whose
        // OperationCanceledException is swallowed without enqueuing RenewCompleted/RenewFaulted. Those two
        // events are the only other places that clear _renewing, so without this reset the guard would stay
        // set and MaybeRenew would never renew again for the rest of the process.
        _renewing = false;
        _session = null;
        _deps.SetToken(null);
        State = State with
        {
            Phase = ApplicationPhase.Login,
            LoginStep = LoginStep.Username,
            PendingUsername = null,
            SigningIn = false,
            Input = TerminalInputState.BeginLogin(),
            IsOffline = false,
            IsBuffering = false,
            IsSaving = false,
            StatusMessage = "Session expired. Please sign in again.",
        };
    }

    private void BeginOnlineStartup()
    {
        // Open a fresh per-session cancellation scope for this sign-in's REST activity; a prior scope (from
        // a session that ended in a 401) is already cancelled, so cancel-and-dispose it before replacing.
        _sessionCts?.Cancel();
        _sessionCts?.Dispose();
        _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(_appToken);
        State = State with { IsBuffering = true, IsOffline = false };
        _ = StartHubAndLoadAsync(++_snapshotGeneration);
    }

    private void RestorePersistedOutbox()
    {
        IReadOnlyList<TerminalTask> outbox;
        try
        {
            outbox = _deps.StateStore.Load()?.Outbox ?? [];
        }
        catch (TerminalStateException)
        {
            return;
        }

        _preservedOutbox = null;
        if (outbox.Count > 0)
        {
            RunSyncEffects(_sync.RestoreOutbox(outbox));
        }
    }

    private void ApplyReconciliation(ReconciliationResult result)
    {
        if (result.TasksToApply.Count > 0 || result.KeepUids is not null)
        {
            State = State with { Cache = MergeIncoming(State.Cache, result.TasksToApply, result.KeepUids) };
        }

        if (result.TasksConflicted.Count > 0)
        {
            State = State with { Notification = BuildConflictNotice(result.TasksConflicted) };
        }
    }

    private void PersistState()
    {
        // Never write over a state file we could not load (malformed or newer-than-supported): doing so
        // would discard the very cache/outbox the blocking error told the user to fix or remove.
        if (_stateLoadFailed)
        {
            return;
        }

        var snapshot = new PersistedTerminalState
        {
            DataOwner = _dataOwner,
            CachedTasks = State.Cache,
            Outbox = _preservedOutbox ?? _sync.BuildOutboxContents(),
            ShowCompleted = State.ShowCompleted,
        };

        try
        {
            _deps.StateStore.Save(snapshot);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // Best-effort persistence; a transient disk error must not crash the interactive loop.
        }
    }

    private void Render()
    {
        // Below the minimum size the renderer shows the resize screen instead of the (unfittable) frame,
        // but Help stays visible there because the resize screen advertises '? help': the input gate keeps
        // every other mode out of this state, so only Help suppresses the resize overlay.
        var resizeRequired = State.Phase == ApplicationPhase.Ready
            && ViewportState.IsResizeRequired(State.Width, State.Height)
            && State.Input.Mode != TerminalUiMode.Help;
        _deps.Renderer.Render(BuildViewModel(), State.Width, State.Height, resizeRequired);
    }

    private TerminalViewModel BuildViewModel()
    {
        return new TerminalViewModel
        {
            Overview = State.Projection,
            Focus = State.Focus,
            Today = _deps.LocalDate.Today,
            ShowCompleted = State.ShowCompleted,
            IsOffline = State.IsOffline,
            Notification = State.StatusMessage ?? State.Notification,
            ProfileName = _deps.ProfileName,
            Status = BuildStatus(),
        };
    }

    private TerminalStatus BuildStatus()
    {
        if (State.Phase == ApplicationPhase.Login)
        {
            return new TerminalStatus
            {
                Kind = TerminalStatusKind.Login,
                Prompt = State.LoginStep == LoginStep.Username ? "Username" : "Password",
                Input = State.Input.Editor.Text,
                CursorPosition = State.Input.Editor.Cursor,
                MaskInput = State.LoginStep == LoginStep.Password,
            };
        }

        if (State.Phase == ApplicationPhase.ConfirmDataReset)
        {
            return new TerminalStatus
            {
                Kind = TerminalStatusKind.Confirmation,
                Prompt = $"Sign in as '{State.ConfirmUser}' and clear local data for '{State.ConfirmOwner}'? (y/n)",
            };
        }

        return State.Input.Mode switch
        {
            TerminalUiMode.Editor => new TerminalStatus
            {
                Kind = TerminalStatusKind.Editor,
                Prompt = EditorPrompt(State.Input.Purpose),
                Input = State.Input.Editor.Text,
                CursorPosition = State.Input.Editor.Cursor,
                Hint = State.Input.Feedback,
            },
            TerminalUiMode.MaskedLogin => new TerminalStatus
            {
                Kind = TerminalStatusKind.Login,
                Prompt = "Password",
                Input = State.Input.Editor.Text,
                CursorPosition = State.Input.Editor.Cursor,
                MaskInput = true,
            },
            TerminalUiMode.DeleteConfirmation => new TerminalStatus
            {
                Kind = TerminalStatusKind.Confirmation,
                Prompt = $"Delete '{FocusedTitle()}'? (y/n)",
            },
            TerminalUiMode.Help => new TerminalStatus { Kind = TerminalStatusKind.Help },
            TerminalUiMode.Normal => new TerminalStatus { Kind = TerminalStatusKind.Normal },
            TerminalUiMode.ResizeRequired => new TerminalStatus { Kind = TerminalStatusKind.Normal },
            _ => new TerminalStatus { Kind = TerminalStatusKind.Normal },
        };
    }

    private void StartProducers()
    {
        _ = RunKeyReaderAsync();
        if (_deps.StartBackgroundTimers)
        {
            _ = RunRenewTimerAsync();
            _ = RunResizeTimerAsync();
        }
    }

    private async Task RunKeyReaderAsync()
    {
        while (!_appToken.IsCancellationRequested)
        {
            ConsoleKeyInfo? key;
            try
            {
                key = await _deps.Keys.ReadKeyAsync(_appToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (key is null || !Enqueue(ApplicationEvent.KeyPressed(key.Value)))
            {
                return;
            }
        }
    }

    private async Task RunRenewTimerAsync()
    {
        while (!_appToken.IsCancellationRequested)
        {
            try
            {
                await _deps.Delay(_deps.RenewInterval, _appToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            Enqueue(ApplicationEvent.RenewTick);
        }
    }

    private async Task RunResizeTimerAsync()
    {
        var (lastWidth, lastHeight) = _deps.ReadDimensions();
        while (!_appToken.IsCancellationRequested)
        {
            try
            {
                await _deps.Delay(_deps.ResizePollInterval, _appToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            var (width, height) = _deps.ReadDimensions();
            if (width != lastWidth || height != lastHeight)
            {
                lastWidth = width;
                lastHeight = height;
                Enqueue(ApplicationEvent.Resized(width, height));
            }
        }
    }

    private async Task StartHubAndLoadAsync(int generation)
    {
        try
        {
            await _hub.StartAsync(_appToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        await LoadSnapshotAsync(generation);
    }

    // Starts a fresh snapshot load, tagging it with the next generation so an older overlapping load whose
    // response arrives later is discarded rather than re-reconciled over newer state. Called only from the
    // single-reader loop (reload retry tick, hub Reconnected), so the counter is never touched off-thread.
    private void StartSnapshotLoad()
    {
        _ = LoadSnapshotAsync(++_snapshotGeneration);
    }

    private async Task LoadSnapshotAsync(int generation)
    {
        try
        {
            var tasks = await _deps.Tasks.LoadTasksAsync(SessionToken);
            Enqueue(ApplicationEvent.SnapshotLoaded(tasks, generation));
        }
        catch (TerminalApiException exception)
        {
            Enqueue(ApplicationEvent.SnapshotFailed(
                exception.Kind == TerminalApiErrorKind.Unauthorized, generation));
        }
        catch (OperationCanceledException)
        {
            // Shutting down; no snapshot needed.
        }
    }

    private void StartSaveBatch(IReadOnlyList<TerminalTask> batch)
    {
        _ = SaveBatchAsync(batch);
    }

    private async Task SaveBatchAsync(IReadOnlyList<TerminalTask> batch)
    {
        try
        {
            var saved = await _deps.Tasks.SaveTasksAsync(batch, SessionToken);
            Enqueue(ApplicationEvent.SaveCompleted(saved));
        }
        catch (TerminalApiException exception)
        {
            Enqueue(ApplicationEvent.SaveFaulted(exception.Kind == TerminalApiErrorKind.Unauthorized));
        }
        catch (OperationCanceledException)
        {
            // Shutting down; the batch stays in the persisted outbox for the next run.
        }
    }

    private void StartSignIn(string username, string password)
    {
        _ = SignInAsync(username, password);
    }

    private async Task SignInAsync(string username, string password)
    {
        try
        {
            var outcome = await _deps.Auth.SignInAsync(username, password, _appToken);
            Enqueue(ApplicationEvent.SignInCompleted(outcome));
        }
        catch (TerminalApiException exception)
        {
            Enqueue(ApplicationEvent.SignInFaulted(FriendlyApiMessage(exception)));
        }
        catch (OperationCanceledException)
        {
            // Shutting down mid sign-in.
        }
    }

    private async Task RenewAsync()
    {
        try
        {
            var session = await _deps.Auth.RenewTokenAsync(SessionToken);
            Enqueue(ApplicationEvent.RenewCompleted(session));
        }
        catch (TerminalApiException exception)
        {
            Enqueue(ApplicationEvent.RenewFaulted(exception.Kind == TerminalApiErrorKind.Unauthorized));
        }
        catch (OperationCanceledException)
        {
            // Shutting down; renewal will resume next run.
        }
    }

    private void StartReconnect()
    {
        _ = ReconnectAsync();
    }

    private async Task ReconnectAsync()
    {
        try
        {
            await _hub.ReconnectNowAsync(_appToken);
        }
        catch (OperationCanceledException)
        {
            // Shutting down.
        }
        catch (ObjectDisposedException)
        {
            // Hub already torn down.
        }
    }

    private void StartDelayedTick(TimeSpan delay, ApplicationEvent tick)
    {
        _ = DelayedTickAsync(delay, tick);
    }

    private async Task DelayedTickAsync(TimeSpan delay, ApplicationEvent tick)
    {
        try
        {
            await _deps.Delay(delay, _appToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        Enqueue(tick);
    }

    private async Task StopHubQuietlyAsync()
    {
        try
        {
            await _hub.StopAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Ignored during teardown.
        }
        catch (ObjectDisposedException)
        {
            // Ignored during teardown.
        }
    }

    private async Task ShutdownAsync()
    {
        if (_lifetimeCts is not null)
        {
            await _lifetimeCts.CancelAsync();
        }

        PersistState();

#pragma warning disable CA1031 // Cleanup must restore the terminal whatever hub teardown throws.
        try
        {
            await StopHubQuietlyAsync();
            await _hub.DisposeAsync();
        }
        catch (Exception)
        {
            // Best-effort hub teardown: an exception here must never skip restoring the terminal screen
            // (leaving the alternate screen active) or disposing the cancellation scopes below.
        }
#pragma warning restore CA1031
        finally
        {
            _deps.Renderer.End();
            _lifetimeCts?.Dispose();
            _sessionCts?.Dispose();
        }
    }

    private string? FocusedTitle()
    {
        var uid = State.Focus?.Uid;
        if (uid is null)
        {
            return null;
        }

        return State.Cache
            .FirstOrDefault(task => string.Equals(task.Uid, uid, StringComparison.Ordinal))?.Title;
    }

    private static IReadOnlyList<TerminalTask> ApplyVersions(
        IReadOnlyList<TerminalTask> cache, IReadOnlyList<TerminalTaskVersion> versions)
    {
        if (versions.Count == 0)
        {
            return cache;
        }

        var byUid = versions.ToDictionary(version => version.Uid, version => version.Version, StringComparer.Ordinal);
        return cache
            .Select(task => byUid.TryGetValue(task.Uid, out var version) ? task with { Version = version } : task)
            .ToList();
    }

    private static IReadOnlyList<TerminalTask> MergeIncoming(
        IReadOnlyList<TerminalTask> cache,
        IReadOnlyList<TerminalTask> incoming,
        IReadOnlyList<string>? keepUids)
    {
        var keep = keepUids is null ? null : new HashSet<string>(keepUids, StringComparer.Ordinal);
        var merged = new Dictionary<string, TerminalTask>(StringComparer.Ordinal);

        foreach (var task in cache)
        {
            if (keep is null || keep.Contains(task.Uid))
            {
                merged[task.Uid] = task;
            }
        }

        foreach (var task in incoming)
        {
            merged[task.Uid] = task;
        }

        return [.. merged.Values];
    }

    private static string BuildConflictNotice(IReadOnlyList<TerminalTask> conflicted)
    {
        return conflicted.Count == 1
            ? $"Updated from server: {conflicted[0].Title}"
            : $"{conflicted.Count} tasks were updated from the server.";
    }

    private static string EditorPrompt(EditorPurpose purpose)
    {
        return purpose switch
        {
            EditorPurpose.AddWithFocusDate => "Add",
            EditorPurpose.AddNoDate => "Add (no date)",
            EditorPurpose.Edit => "Edit",
            EditorPurpose.Move => "Move to date",
            EditorPurpose.Login => "Login",
            EditorPurpose.None => string.Empty,
            _ => string.Empty,
        };
    }

    private static string FriendlyApiMessage(TerminalApiException exception)
    {
        return exception.Kind switch
        {
            TerminalApiErrorKind.Transport => "Could not reach the server.",
            TerminalApiErrorKind.Validation => "The server rejected the request.",
            TerminalApiErrorKind.Protocol => "Unexpected server response.",
            TerminalApiErrorKind.Unauthorized => "Authentication failed.",
            _ => "Sign-in failed.",
        };
    }
}
