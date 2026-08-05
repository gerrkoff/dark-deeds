using Spectre.Console.Rendering;

namespace DD.TerminalClient.Details.Ui;

// The Overview content rendered as a flat, top-to-bottom list of single-line renderables plus the line
// metadata for every visible task. Lines contains one renderable per visual line (section titles, day
// headers, week separators, task lines and the collapsed-Routine summary), each exactly one terminal
// line tall, so TaskLines[i].LineIndex indexes directly into Lines. The viewport clips Lines and reserves
// the fixed header and footer around them.
public sealed record RenderedOverview
{
    public IReadOnlyList<IRenderable> Lines { get; init; } = [];

    public IReadOnlyList<TaskLine> TaskLines { get; init; } = [];
}
