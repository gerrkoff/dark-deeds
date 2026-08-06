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

// Exercises the scrollable viewport: ViewportState's pure scroll-into-view maths (content that fits, focus
// above/inside/below the window, offset clamping and the resize threshold) and ViewportRenderable's
// segment clipping (top/middle/bottom windows), style preservation across the clip, focus following,
// continuation indicators, fixed-height reservation that pins the footer, terminal-dimension resolution
// with a profile fallback when the window-size API is unavailable, and the resize-required screen.
public sealed class ViewportRenderableTests
{
    private static readonly DateOnly Monday = new(2024, 11, 4);

    [Fact]
    public void Calculate_ContentFitsViewport_DoesNotScroll()
    {
        var state = ViewportState.Calculate(totalLines: 5, viewportHeight: 10, focusedLine: 2, previousOffset: 0);

        Assert.False(state.Scrollable);
        Assert.Equal(0, state.Offset);
        Assert.Equal(5, state.WindowHeight);
        Assert.False(state.HasAbove);
        Assert.False(state.HasBelow);
    }

    [Fact]
    public void Calculate_FocusBelowWindow_ScrollsSoFocusIsVisible()
    {
        var state = ViewportState.Calculate(totalLines: 30, viewportHeight: 10, focusedLine: 25, previousOffset: 0);

        Assert.True(state.Scrollable);
        Assert.InRange(25, state.Offset, state.Offset + state.WindowHeight - 1);
        Assert.Equal(25 - state.WindowHeight + 1, state.Offset);
        Assert.True(state.HasAbove);
    }

    [Fact]
    public void Calculate_FocusAtLastLine_ClampsToBottomAndHidesBelowIndicator()
    {
        var state = ViewportState.Calculate(totalLines: 30, viewportHeight: 10, focusedLine: 29, previousOffset: 0);

        Assert.Equal(30 - state.WindowHeight, state.Offset);
        Assert.True(state.HasAbove);
        Assert.False(state.HasBelow);
    }

    [Fact]
    public void Calculate_FocusAboveWindow_ScrollsUpToFocus()
    {
        var state = ViewportState.Calculate(totalLines: 30, viewportHeight: 10, focusedLine: 3, previousOffset: 20);

        Assert.Equal(3, state.Offset);
        Assert.InRange(3, state.Offset, state.Offset + state.WindowHeight - 1);
    }

    [Fact]
    public void Calculate_FocusAtFirstLine_ShowsBelowIndicatorOnly()
    {
        var state = ViewportState.Calculate(totalLines: 30, viewportHeight: 10, focusedLine: 0, previousOffset: 12);

        Assert.Equal(0, state.Offset);
        Assert.False(state.HasAbove);
        Assert.True(state.HasBelow);
    }

    [Fact]
    public void Calculate_FocusInsideWindow_PreservesPreviousOffset()
    {
        var state = ViewportState.Calculate(totalLines: 30, viewportHeight: 10, focusedLine: 12, previousOffset: 10);

        Assert.Equal(10, state.Offset);
        Assert.True(state.HasAbove);
        Assert.True(state.HasBelow);
    }

    [Fact]
    public void Calculate_NoFocus_ClampsPreviousOffsetIntoRange()
    {
        var state = ViewportState.Calculate(totalLines: 30, viewportHeight: 10, focusedLine: null, previousOffset: 999);

        Assert.Equal(30 - state.WindowHeight, state.Offset);
        Assert.True(state.Scrollable);
    }

    [Theory]
    [InlineData(74, 30, true)]
    [InlineData(75, 29, true)]
    [InlineData(74, 29, true)]
    [InlineData(75, 30, false)]
    [InlineData(200, 60, false)]
    public void IsResizeRequired_BelowMinimum_IsTrue(int width, int height, bool expected)
    {
        Assert.Equal(expected, ViewportState.IsResizeRequired(width, height));
    }

    [Theory]
    [InlineData(30, 6, 24)]
    [InlineData(40, 5, 35)]
    [InlineData(5, 6, 1)]
    public void ContentHeight_ReservesChromeWithFloorOfOne(int terminalHeight, int reserved, int expected)
    {
        Assert.Equal(expected, ViewportState.ContentHeight(terminalHeight, reserved));
    }

    [Fact]
    public void Render_TopFocus_ClipsBottomAndShowsMoreBelow()
    {
        var projection = Project(NoDateTasks(25));
        var (viewport, _, _) = Build(Vm(projection, focus: FocusFor(projection, "u00")), width: 120, viewportHeight: 10);
        var console = Plain(120);

        console.Write(viewport);
        var output = console.Output;

        Assert.Contains("T00", output, StringComparison.Ordinal);
        Assert.Contains("T06", output, StringComparison.Ordinal);
        Assert.DoesNotContain("T07", output, StringComparison.Ordinal);
        Assert.DoesNotContain("T24", output, StringComparison.Ordinal);
        Assert.Contains("more below", output, StringComparison.Ordinal);
        Assert.DoesNotContain("more above", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_BottomFocus_ClipsTopAndShowsMoreAbove()
    {
        var projection = Project(
            [.. NoDateTasks(24), Task("u24", date: Monday.AddDays(13), title: "T24")]);
        var (viewport, _, _) = Build(Vm(projection, focus: FocusFor(projection, "u24")), width: 120, viewportHeight: 10);
        var console = Plain(120);

        console.Write(viewport);
        var output = console.Output;

        Assert.Contains("T24", output, StringComparison.Ordinal);
        Assert.DoesNotContain("T00", output, StringComparison.Ordinal);
        Assert.DoesNotContain("No Date", output, StringComparison.Ordinal);
        Assert.Contains("more above", output, StringComparison.Ordinal);
        Assert.DoesNotContain("more below", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_MiddleWindow_ClipsBothEndsAndShowsBothIndicators()
    {
        var projection = Project(NoDateTasks(25));
        var (viewport, _, _) = Build(
            Vm(projection, focus: FocusFor(projection, "u12")),
            width: 120,
            viewportHeight: 10,
            previousOffset: 10);
        var console = Plain(120);

        console.Write(viewport);
        var output = console.Output;

        Assert.Contains("T09", output, StringComparison.Ordinal);
        Assert.Contains("T16", output, StringComparison.Ordinal);
        Assert.DoesNotContain("T08", output, StringComparison.Ordinal);
        Assert.DoesNotContain("T17", output, StringComparison.Ordinal);
        Assert.Contains("more above", output, StringComparison.Ordinal);
        Assert.Contains("more below", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_ScrolledFocusedLine_PreservesItsStyle()
    {
        var projection = Project(NoDateTasks(25));
        var (viewport, state, _) = Build(Vm(projection, focus: FocusFor(projection, "u20")), width: 120, viewportHeight: 10);

        Assert.True(state.Offset > 0, "expected the focused task to require scrolling");
        var output = Ansi(viewport, 120);
        var console = Plain(120);
        console.Write(viewport);

        Assert.Contains("T20", output, StringComparison.Ordinal);
        Assert.Contains("> T20", console.Output, StringComparison.Ordinal);
        Assert.Contains("[38;5;15m", output, StringComparison.Ordinal);
        Assert.DoesNotContain("T00", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_FocusChange_ScrollsPreviouslyClippedTaskIntoView()
    {
        var projection = Project(NoDateTasks(25));

        var (top, _, _) = Build(Vm(projection, focus: FocusFor(projection, "u00")), width: 120, viewportHeight: 10);
        var topConsole = Plain(120);
        topConsole.Write(top);
        Assert.DoesNotContain("T24", topConsole.Output, StringComparison.Ordinal);

        var (bottom, _, _) = Build(Vm(projection, focus: FocusFor(projection, "u24")), width: 120, viewportHeight: 10);
        var bottomConsole = Plain(120);
        bottomConsole.Write(bottom);
        Assert.Contains("T24", bottomConsole.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("T00", bottomConsole.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_ReturningToFirstNoDateTask_RestoresCardHeader()
    {
        var projection = Project(NoDateTasks(25));
        var (viewport, state, _) = Build(
            Vm(projection, focus: FocusFor(projection, "u00")),
            width: 120,
            viewportHeight: 10,
            previousOffset: 12);
        var console = Plain(120);

        console.Write(viewport);

        Assert.Equal(0, state.Offset);
        Assert.Contains("No Date", console.Output, StringComparison.Ordinal);
        Assert.Contains("T00", console.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_ContentShorterThanViewport_ShowsEverythingWithoutIndicators()
    {
        var projection = Project(NoDateTasks(3));
        var (viewport, state, _) = Build(Vm(projection), width: 120, viewportHeight: 40);
        var console = Plain(120);

        console.Write(viewport);
        var output = console.Output;

        Assert.False(state.Scrollable);
        Assert.Contains("T00", output, StringComparison.Ordinal);
        Assert.Contains("T01", output, StringComparison.Ordinal);
        Assert.Contains("T02", output, StringComparison.Ordinal);
        Assert.DoesNotContain("more above", output, StringComparison.Ordinal);
        Assert.DoesNotContain("more below", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_ReservesFixedHeight_PinsTrailingFooter()
    {
        var projection = Project(NoDateTasks(3));
        var (viewport, state, _) = Build(Vm(projection), width: 120, viewportHeight: 12);
        var console = Plain(120);

        console.Write(new Rows(viewport, new Text("FOOTER_SENTINEL")));

        var footerIndex = -1;
        for (var i = 0; i < console.Lines.Count; i++)
        {
            if (console.Lines[i].Contains("FOOTER_SENTINEL", StringComparison.Ordinal))
            {
                footerIndex = i;
                break;
            }
        }

        Assert.Equal(state.ViewportHeight, footerIndex);
    }

    [Fact]
    public void Render_DimensionChange_RecomputesClipping()
    {
        var projection = Project(NoDateTasks(25));

        var (tall, tallState, _) = Build(Vm(projection), width: 120, viewportHeight: 60);
        var tallConsole = Plain(120);
        tallConsole.Write(tall);
        Assert.False(tallState.Scrollable);
        Assert.Contains("T24", tallConsole.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("more below", tallConsole.Output, StringComparison.Ordinal);

        var (shortViewport, shortState, _) = Build(Vm(projection), width: 120, viewportHeight: 10);
        var shortConsole = Plain(120);
        shortConsole.Write(shortViewport);
        Assert.True(shortState.Scrollable);
        Assert.DoesNotContain("T24", shortConsole.Output, StringComparison.Ordinal);
        Assert.Contains("more below", shortConsole.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void Render_WidthChange_RetruncatesLineMetadataToOneLine()
    {
        var title = new string('a', 60);
        var projection = Project(Task("u0", title: title));

        var wide = OverviewRenderer.Render(Vm(projection), 120);
        var narrow = OverviewRenderer.Render(Vm(projection), 40);

        Assert.Equal(wide.TaskLines.Count, narrow.TaskLines.Count);

        var wideConsole = Plain(120);
        wideConsole.Write(new Rows(wide.Lines));
        var narrowConsole = Plain(40);
        narrowConsole.Write(new Rows(narrow.Lines));

        Assert.DoesNotContain('\u2026', wideConsole.Lines[wide.TaskLines[0].LineIndex]);
        Assert.Contains('\u2026', narrowConsole.Lines[narrow.TaskLines[0].LineIndex]);
        Assert.True(narrowConsole.Lines[narrow.TaskLines[0].LineIndex].GetCellWidth() <= 40);
    }

    [Fact]
    public void ResolveDimensions_WindowSizeUnavailable_FallsBackToProfile()
    {
        var console = Plain(90);
        console.Profile.Height = 42;

        var (width, height) = ViewportRenderable.ResolveDimensions(console, () => null);

        Assert.Equal(90, width);
        Assert.Equal(42, height);
    }

    [Fact]
    public void ResolveDimensions_WindowSizeAvailable_UsesReportedSize()
    {
        var console = Plain(90);
        console.Profile.Height = 42;

        var (width, height) = ViewportRenderable.ResolveDimensions(console, () => (200, 55));

        Assert.Equal(200, width);
        Assert.Equal(55, height);
    }

    [Fact]
    public void ResolveDimensions_NonpositiveDimension_FallsBackPerDimension()
    {
        var console = Plain(90);
        console.Profile.Height = 42;

        var (width, height) = ViewportRenderable.ResolveDimensions(console, () => (0, 55));

        Assert.Equal(90, width);
        Assert.Equal(55, height);
    }

    [Fact]
    public void ReadWindowSize_WhenUnavailable_ReturnsNullWithoutThrowing()
    {
        var size = ViewportRenderable.ReadWindowSize();

        Assert.True(size is null || (size.Value.Width > 0 && size.Value.Height > 0));
    }

    [Fact]
    public void FindFocusedLine_WithoutFocus_ReturnsNull()
    {
        var rendered = OverviewRenderer.Render(Vm(Project(NoDateTasks(3))), 120);

        Assert.Null(ViewportRenderable.FindFocusedLine(rendered, null));
    }

    [Fact]
    public void RenderResizeRequired_StatesRequiredSizeAndAcceptedKeys()
    {
        var console = Plain(90);

        console.Write(ViewportRenderable.RenderResizeRequired(80, 20));
        var output = console.Output;

        Assert.Contains("80x20", output, StringComparison.Ordinal);
        Assert.Contains(
            $"{ViewportState.MinimumWidth}x{ViewportState.MinimumHeight}",
            output,
            StringComparison.Ordinal);
        Assert.Contains("Syncing continues", output, StringComparison.Ordinal);
        Assert.Contains("q quit", output, StringComparison.Ordinal);
    }

    private static (ViewportRenderable Viewport, ViewportState State, RenderedOverview Rendered) Build(
        TerminalViewModel model,
        int width,
        int viewportHeight,
        int previousOffset = 0)
    {
        var rendered = OverviewRenderer.Render(model, width);
        var focusedLine = ViewportRenderable.FindFocusedLine(rendered, model.Focus, previousOffset);
        var state = ViewportState.Calculate(rendered.Lines.Count, viewportHeight, focusedLine, previousOffset);
        return (new ViewportRenderable(new Rows(rendered.Lines), state), state, rendered);
    }

    private static TerminalTask[] NoDateTasks(int count)
    {
        var tasks = new TerminalTask[count];
        for (var i = 0; i < count; i++)
        {
            tasks[i] = Task($"u{i:D2}", order: i, title: $"T{i:D2}");
        }

        return tasks;
    }

    private static OverviewProjection Project(params TerminalTask[] tasks)
    {
        var provider = new Mock<ILocalDateProvider>();
        provider.SetupGet(x => x.Today).Returns(Monday);
        var service = new OverviewProjectionService(provider.Object);
        return service.Project(tasks, showCompleted: false, routineShownDates: new HashSet<DateOnly>());
    }

    private static TerminalViewModel Vm(OverviewProjection projection, TaskFocus? focus = null)
    {
        return new TerminalViewModel
        {
            Overview = projection,
            Focus = focus,
            Today = Monday,
            ShowCompleted = false,
            IsOffline = false,
            Notification = null,
            ProfileName = string.Empty,
            Status = new TerminalStatus(),
        };
    }

    private static TaskFocus FocusFor(OverviewProjection projection, string uid)
    {
        var days = new List<OverviewDay> { projection.NoDate };
        days.AddRange(projection.Overdue);
        days.AddRange(projection.Current);
        days.AddRange(projection.Future);

        foreach (var overviewTask in days.SelectMany(day => day.Tasks))
        {
            if (string.Equals(overviewTask.Task.Uid, uid, StringComparison.Ordinal))
            {
                return new TaskFocus { Uid = uid, Address = overviewTask.Address };
            }
        }

        throw new InvalidOperationException($"Task {uid} not found");
    }

    private static TestConsole Plain(int width)
    {
        var console = new TestConsole();
        console.Profile.Width = width;
        return console;
    }

    private static string Ansi(IRenderable renderable, int width)
    {
        var console = new TestConsole().EmitAnsiSequences();
        console.Profile.Width = width;
        console.Write(renderable);
        return console.Output;
    }

    private static TerminalTask Task(
        string uid,
        int order = 0,
        string title = "Task",
        DateOnly? date = null)
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = title,
            Date = date,
            Time = null,
            Order = order,
            Completed = false,
            Deleted = false,
            Type = TerminalTaskType.Simple,
            IsProbable = false,
            Version = 0,
        };
    }
}
