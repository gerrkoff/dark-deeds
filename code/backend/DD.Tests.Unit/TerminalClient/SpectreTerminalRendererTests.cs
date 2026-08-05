using DD.TerminalClient;
using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Application;
using Spectre.Console;
using Spectre.Console.Testing;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

public sealed class SpectreTerminalRendererTests
{
    [Fact]
    public void Render_WrapsFrameInSynchronizedUpdate()
    {
        var console = new TestConsole().EmitAnsiSequences();
        console.Profile.Width = 100;
        console.Profile.Height = 30;
        var renderer = new SpectreTerminalRenderer(console);

        renderer.Render(
            new TerminalViewModel
            {
                Overview = new ApplicationState().Projection,
                Today = new DateOnly(2024, 11, 4),
            },
            100,
            30,
            resizeRequired: false);

        var begin = console.Output.IndexOf("\u001b[?2026h", StringComparison.Ordinal);
        var end = console.Output.IndexOf("\u001b[?2026l", StringComparison.Ordinal);
        Assert.True(begin >= 0);
        Assert.True(end > begin);
    }

    [Fact]
    public void WidthConstrainedRenderable_LeavesRightmostColumnUnused()
    {
        var console = new TestConsole();
        console.Profile.Width = 10;

        console.Write(new SpectreTerminalRenderer.WidthConstrainedRenderable(new Rule(), width: 9));

        Assert.Equal(9, Assert.Single(console.Lines).Length);
    }

    [Fact]
    public void WidthConstrainedRenderable_OmitsTrailingLineBreak()
    {
        var console = new TestConsole();
        console.Profile.Width = 10;

        console.Write(new SpectreTerminalRenderer.WidthConstrainedRenderable(
            new Rows(new Text("header"), new Text("footer")),
            width: 9));

        Assert.False(console.Output.EndsWith('\n'));
    }
}
