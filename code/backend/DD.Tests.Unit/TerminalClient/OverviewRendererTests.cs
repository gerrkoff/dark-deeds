using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Time;
using Moq;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Exercises the Spectre terminal frame: TerminalStyles.ForTask style composition, OverviewRenderer
// single-line-per-task rendering with deterministic line metadata and markup-safe/Unicode/long titles,
// and TerminalFrame's header, footer and content for every UI mode (normal, editor, login,
// confirmation, help, offline, conflict and empty-state). Style coverage is asserted both purely (the
// composed Style) and end to end (the emitted ANSI SGR codes) through a TestConsole.
public sealed class OverviewRendererTests
{
    private static readonly DateOnly Monday = new(2024, 11, 4);

    [Fact]
    public void Render_Empty_ShowsEmptyState()
    {
        var console = Plain(80);
        console.Write(TerminalFrame.Render(Vm(Project()), 80));

        Assert.Contains("No tasks to show", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_Normal_ShowsSectionsHeadersTitlesAndHints()
    {
        var projection = Project(
            Task("nodate", title: "Buy milk"),
            Task("overdue", date: Monday.AddDays(-1), title: "Old thing"),
            Task("current", date: Monday, title: "Today thing"),
            Task("future", date: Monday.AddDays(20), title: "Later thing"));
        var console = Plain(80);
        var contentConsole = Plain(80);
        var model = Vm(projection);

        console.Write(TerminalFrame.Render(model, 80));
        contentConsole.Write(TerminalFrame.RenderContent(model, 80));
        var output = console.Output;
        var contentOutput = contentConsole.Output;

        Assert.Contains("Dark Deeds", output, StringComparison.Ordinal);
        Assert.Contains("No Date", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Expired", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Current", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Future", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Buy milk", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Old thing", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Today thing", contentOutput, StringComparison.Ordinal);
        Assert.Contains("Later thing", contentOutput, StringComparison.Ordinal);
        Assert.Contains("11/04 Mon", contentOutput, StringComparison.Ordinal);
        Assert.Contains("? help", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_Dense_ProducesOneDeterministicLinePerTask()
    {
        TerminalTask[] tasks =
        [
            Task("n1", title: "N1"),
            Task("n2", title: "N2"),
            Task("o1", date: Monday.AddDays(-2), title: "O1"),
            Task("c1", date: Monday, order: 1, title: "C1"),
            Task("c2", date: Monday, order: 2, title: "C2"),
            Task("f1", date: Monday.AddDays(15), title: "F1"),
        ];
        var projection = Project(tasks);
        var rendered = OverviewRenderer.Render(Vm(projection), 80);

        Assert.Equal(6, rendered.TaskLines.Count);

        var console = Plain(80);
        console.Write(new Rows(rendered.Lines));

        foreach (var (address, task) in Flatten(projection))
        {
            var taskLine = rendered.TaskLines.Single(line => line.Address == address);
            Assert.Contains(task.Title, console.Lines[taskLine.LineIndex], StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Render_Cards_AreSeparatedByBlankLines()
    {
        var projection = Project(
            Task("nodate"),
            Task("monday", date: Monday),
            Task("tuesday", date: Monday.AddDays(1)),
            Task("future", date: Monday.AddDays(14)));

        var rendered = OverviewRenderer.Render(Vm(projection), 80);
        var mondayLine = rendered.TaskLines.Single(line =>
            line.Address is { Section: OverviewSection.Current, Column: 0 });
        var tuesdayLine = rendered.TaskLines.Single(line =>
            line.Address is { Section: OverviewSection.Current, Column: 1 });

        Assert.Equal(1, rendered.TaskLines.Single(line => line.Address.Section == OverviewSection.NoDate).LineIndex);
        Assert.Equal(5, mondayLine.LineIndex);
        Assert.Equal(8, tuesdayLine.LineIndex);
        Assert.Equal(12, rendered.TaskLines.Single(line => line.Address.Section == OverviewSection.Future).LineIndex);
        Assert.Equal(string.Empty, RenderLine(rendered.Lines[2], 80));
        Assert.Equal(string.Empty, RenderLine(rendered.Lines[6], 80));
        Assert.Equal(string.Empty, RenderLine(rendered.Lines[9], 80));
        Assert.Equal(13, rendered.Lines.Count);
    }

    [Fact]
    public void Render_LongTitle_IsEllipsizedToOneLine()
    {
        var projection = Project(Task("long", title: new string('a', 200)));
        var rendered = OverviewRenderer.Render(Vm(projection), 30);
        var console = Plain(30);
        console.Write(new Rows(rendered.Lines));

        var line = console.Lines[rendered.TaskLines.Single().LineIndex];
        Assert.Contains('\u2026', line);
        Assert.True(line.GetCellWidth() <= 30, $"line width {line.GetCellWidth()} exceeded 30");
    }

    [Fact]
    public void Render_MarkupLikeTitle_IsEscapedNotInterpreted()
    {
        var projection = Project(Task("markup", title: "[red]danger[/] {0}"));
        var console = Plain(80);

        console.Write(new Rows(OverviewRenderer.Render(Vm(projection), 80).Lines));

        Assert.Contains("[red]danger[/] {0}", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_UnicodeTitle_IsPreservedOnOneLine()
    {
        var projection = Project(Task("unicode", title: "\u4e70\u725b\u5976 \U0001F95B milk"));
        var rendered = OverviewRenderer.Render(Vm(projection), 40);
        var console = Plain(40);
        console.Write(new Rows(rendered.Lines));

        var line = console.Lines[rendered.TaskLines.Single().LineIndex];
        Assert.Contains("\u4e70\u725b\u5976", line, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_TimedTask_ShowsTimeLabel()
    {
        var projection = Project(Task("timed", date: Monday, time: 9 * 60 + 30, title: "Standup"));
        var console = Plain(80);

        console.Write(new Rows(OverviewRenderer.Render(Vm(projection), 80).Lines));

        Assert.Contains("09:30", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_NonTodayDayHeader_IsBlueWithoutEdgeMarkers()
    {
        var projection = Project(Task("tomorrow", date: Monday.AddDays(1), title: "Later"));

        var output = Ansi(new Rows(OverviewRenderer.Render(Vm(projection), 80).Lines), 80);

        Assert.Contains("11/05 Tue", output, StringComparison.Ordinal);
        Assert.DoesNotContain("--", output, StringComparison.Ordinal);
        Assert.Contains("[1;38;5;12m", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_AdditionalTask_HasExtraLeftIndent()
    {
        var simple = Task("simple", date: Monday, title: "Simple");
        var additional = Task("additional", date: Monday, type: TerminalTaskType.Additional, title: "Additional");

        Assert.Equal("  ", OverviewRenderer.BuildTaskPrefix(simple, isSelected: false));
        Assert.Equal(new string(' ', 14), OverviewRenderer.BuildTaskPrefix(additional, isSelected: false));
        Assert.Equal("> " + new string(' ', 12), OverviewRenderer.BuildTaskPrefix(additional, isSelected: true));
    }

    [Fact]
    public void Render_RoutineCollapsed_ShowsCollapsedCount()
    {
        var projection = ProjectRaw(
            [Task("routine", date: Monday, type: TerminalTaskType.Routine, title: "Vitamins")],
            showCompleted: false,
            routineShownDates: new HashSet<DateOnly>());
        var console = Plain(80);

        console.Write(new Rows(OverviewRenderer.Render(Vm(projection), 80).Lines));

        Assert.Contains("+1 routine", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_CompletedRoutinesCollapsed_ShowsZeroCount()
    {
        var projection = ProjectRaw(
            [
                Task("routine-1", date: Monday, completed: true, type: TerminalTaskType.Routine),
                Task("routine-2", date: Monday, completed: true, type: TerminalTaskType.Routine),
            ],
            showCompleted: false,
            routineShownDates: new HashSet<DateOnly>());
        var console = Plain(80);
        var rendered = new Rows(OverviewRenderer.Render(Vm(projection), 80).Lines);

        console.Write(rendered);
        var ansi = Ansi(rendered, 80);

        Assert.Contains("+0 routine", console.Output, StringComparison.Ordinal);
        Assert.Contains("[2;38;5;8m", ansi, StringComparison.Ordinal);
        Assert.DoesNotContain("[9;", ansi, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(TerminalTaskType.Simple, false, false)]
    [InlineData(TerminalTaskType.Additional, false, false)]
    [InlineData(TerminalTaskType.Routine, false, false)]
    [InlineData(TerminalTaskType.Weekly, false, false)]
    public void ForTask_TypeStyles_UseCurrentForegroundAndDecorationRules(
        TerminalTaskType type,
        bool completed,
        bool probable)
    {
        var style = TerminalStyles.ForTask(
            Task("x", type: type, completed: completed, probable: probable),
            isSelected: false);

        var expectedForeground = type == TerminalTaskType.Simple ? Color.Default : Color.Grey;
        Assert.Equal(expectedForeground, style.Foreground);
        Assert.Equal(type == TerminalTaskType.Additional, style.Decoration.HasFlag(Decoration.Dim));
        Assert.False(style.Decoration.HasFlag(Decoration.Bold));
    }

    [Fact]
    public void ForTask_CompletedAndProbable_ComposeDecorations()
    {
        var completed = TerminalStyles.ForTask(Task("x", completed: true), isSelected: false);
        Assert.True(completed.Decoration.HasFlag(Decoration.Strikethrough));
        Assert.Equal(Color.Grey, completed.Foreground);

        var probable = TerminalStyles.ForTask(Task("x", probable: true), isSelected: false);
        Assert.True(probable.Decoration.HasFlag(Decoration.Italic));
    }

    [Theory]
    [InlineData(TerminalTaskType.Additional, false, false, false, "[2;38;5;8m")]
    [InlineData(TerminalTaskType.Routine, false, false, false, "[38;5;8m")]
    [InlineData(TerminalTaskType.Weekly, false, false, false, "[38;5;8m")]
    [InlineData(TerminalTaskType.Simple, true, false, false, "[2;9;38;5;8m")]
    [InlineData(TerminalTaskType.Simple, false, true, false, "[3m")]
    public void Render_TaskStyle_EmitsExpectedAnsi(
        TerminalTaskType type,
        bool completed,
        bool probable,
        bool selected,
        string expectedSgr)
    {
        var task = Task("styled", date: Monday, type: type, completed: completed, probable: probable, title: "Styled");
        var projection = ProjectRaw([task], showCompleted: true, routineShownDates: new HashSet<DateOnly> { Monday });
        var model = selected
            ? Vm(projection, focus: FocusFor(projection, "styled"))
            : Vm(projection);

        var output = Ansi(new Rows(OverviewRenderer.Render(model, 80).Lines), 80);

        Assert.Contains(expectedSgr, output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_CompletedTask_KeepsMarkerAndIndentWhiteWithoutStrikethrough()
    {
        var task = Task(
            "completed",
            date: Monday,
            completed: true,
            type: TerminalTaskType.Additional,
            title: "Completed");
        var projection = ProjectRaw([task], showCompleted: true, routineShownDates: new HashSet<DateOnly>());
        var model = Vm(projection, focus: FocusFor(projection, "completed"));

        var output = Ansi(new Rows(OverviewRenderer.Render(model, 80).Lines), 80);
        var prefixIndex = output.IndexOf("> " + new string(' ', 12), StringComparison.Ordinal);
        var titleIndex = output.IndexOf("Completed", StringComparison.Ordinal);
        var whiteIndex = output.LastIndexOf("[38;5;15m", prefixIndex, StringComparison.Ordinal);
        var completedStyleIndex = output.LastIndexOf("[2;9;38;5;8m", titleIndex, StringComparison.Ordinal);

        Assert.True(whiteIndex >= 0 && whiteIndex < prefixIndex);
        Assert.True(completedStyleIndex > prefixIndex && completedStyleIndex < titleIndex);
    }

    [Fact]
    public void Render_TodayHeader_IsHighlighted()
    {
        var projection = Project(Task("today", date: Monday, title: "Now"));

        var output = Ansi(new Rows(OverviewRenderer.Render(Vm(projection), 80).Lines), 80);

        Assert.Contains("[1;38;5;11m", output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderFooter_Normal_ShowsSelectedTaskFullText()
    {
        var projection = Project(Task("sel", date: Monday, time: 8 * 60, title: "Long selected title here"));
        var console = Plain(80);

        console.Write(TerminalFrame.RenderFooter(Vm(projection, focus: FocusFor(projection, "sel"))));

        Assert.Contains("Selected:", console.Output, StringComparison.Ordinal);
        Assert.Contains("Long selected title here", console.Output, StringComparison.Ordinal);
        Assert.Contains("08:00", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderFooter_Editor_ShowsPromptAndBuffer()
    {
        var status = new TerminalStatus
        {
            Kind = TerminalStatusKind.Editor,
            Prompt = "Add",
            Input = "buy milk",
            CursorPosition = 3,
        };
        var console = Plain(80);

        console.Write(TerminalFrame.RenderFooter(Vm(Project(), status: status)));

        Assert.Contains("Add", console.Output, StringComparison.Ordinal);
        Assert.Contains("buy milk", console.Output, StringComparison.Ordinal);
        Assert.Contains("Enter save", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderFooter_Login_MasksPassword()
    {
        var status = new TerminalStatus
        {
            Kind = TerminalStatusKind.Login,
            Prompt = "Password",
            Input = "secret",
            CursorPosition = 6,
            MaskInput = true,
        };
        var console = Plain(80);

        console.Write(TerminalFrame.RenderFooter(Vm(Project(), status: status)));

        Assert.Contains("Password", console.Output, StringComparison.Ordinal);
        Assert.Contains("******", console.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("secret", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderFooter_Confirmation_ShowsQuestion()
    {
        var status = new TerminalStatus
        {
            Kind = TerminalStatusKind.Confirmation,
            Prompt = "Delete 'Task 1'? (y/n)",
        };
        var console = Plain(80);

        console.Write(TerminalFrame.RenderFooter(Vm(Project(), status: status)));

        Assert.Contains("Delete 'Task 1'?", console.Output, StringComparison.Ordinal);
        Assert.Contains("y confirm", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderContent_Help_ShowsKeymap()
    {
        var status = new TerminalStatus { Kind = TerminalStatusKind.Help };
        var console = Plain(80);

        console.Write(TerminalFrame.RenderContent(Vm(Project(Task("t", title: "Hidden")), status: status), 80));

        Assert.Contains("Keyboard shortcuts", console.Output, StringComparison.Ordinal);
        Assert.Contains("Quit", console.Output, StringComparison.Ordinal);
        Assert.Contains("Previous / next task", console.Output, StringComparison.Ordinal);
        Assert.Contains("Previous / next day", console.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("Hidden", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderHeader_Offline_ShowsOfflineIndicator()
    {
        var console = Plain(80);

        console.Write(TerminalFrame.RenderHeader(Vm(Project(), offline: true)));

        Assert.Contains("offline", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderFooter_Conflict_ShowsNotification()
    {
        var console = Plain(80);

        console.Write(TerminalFrame.RenderFooter(Vm(Project(), notification: "Task changed on the server")));

        Assert.Contains("Task changed on the server", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void RenderHeader_ShowsProfileAndCompletedIndicator()
    {
        var console = Plain(80);

        console.Write(TerminalFrame.RenderHeader(Vm(Project(), profileName: "production", showCompleted: true)));

        Assert.Contains("production", console.Output, StringComparison.Ordinal);
        Assert.Contains("completed shown", console.Output, StringComparison.Ordinal);
    }

    private static OverviewProjection Project(params TerminalTask[] tasks)
    {
        return ProjectRaw(tasks, showCompleted: false, routineShownDates: new HashSet<DateOnly>());
    }

    private static OverviewProjection ProjectRaw(
        TerminalTask[] tasks,
        bool showCompleted,
        IReadOnlySet<DateOnly> routineShownDates)
    {
        var provider = new Mock<ILocalDateProvider>();
        provider.SetupGet(x => x.Today).Returns(Monday);
        var service = new OverviewProjectionService(provider.Object);
        return service.Project(tasks, showCompleted, routineShownDates);
    }

    private static TerminalViewModel Vm(
        OverviewProjection projection,
        TaskFocus? focus = null,
        TerminalStatus? status = null,
        bool offline = false,
        string? notification = null,
        string profileName = "",
        bool showCompleted = false)
    {
        return new TerminalViewModel
        {
            Overview = projection,
            Focus = focus,
            Today = Monday,
            ShowCompleted = showCompleted,
            IsOffline = offline,
            Notification = notification,
            ProfileName = profileName,
            Status = status ?? new TerminalStatus(),
        };
    }

    private static TaskFocus FocusFor(OverviewProjection projection, string uid)
    {
        var (address, _) = Flatten(projection).Single(pair => string.Equals(pair.Task.Uid, uid, StringComparison.Ordinal));
        return new TaskFocus { Uid = uid, Address = address };
    }

    private static List<(VisualTaskAddress Address, TerminalTask Task)> Flatten(OverviewProjection projection)
    {
        var result = new List<(VisualTaskAddress, TerminalTask)>();
        var days = new List<OverviewDay> { projection.NoDate };
        days.AddRange(projection.Overdue);
        days.AddRange(projection.Current);
        days.AddRange(projection.Future);

        foreach (var day in days)
        {
            foreach (var overviewTask in day.Tasks)
            {
                result.Add((overviewTask.Address, overviewTask.Task));
            }
        }

        return result;
    }

    private static TestConsole Plain(int width)
    {
        var console = new TestConsole();
        console.Profile.Width = width;
        console.Profile.Height = 100;
        return console;
    }

    private static string Ansi(IRenderable renderable, int width)
    {
        var console = new TestConsole().EmitAnsiSequences();
        console.Profile.Width = width;
        console.Profile.Height = 100;
        console.Write(renderable);
        return console.Output;
    }

    private static string RenderLine(IRenderable renderable, int width)
    {
        var console = Plain(width);
        console.Write(renderable);
        return console.Output;
    }

    private static TerminalTask Task(
        string uid,
        DateOnly? date = null,
        int? time = null,
        int order = 0,
        bool completed = false,
        bool probable = false,
        TerminalTaskType type = TerminalTaskType.Simple,
        string title = "Task")
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = title,
            Date = date,
            Time = time,
            Order = order,
            Completed = completed,
            Deleted = false,
            Type = type,
            IsProbable = probable,
            Version = 0,
        };
    }
}
