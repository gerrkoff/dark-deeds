namespace DD.TerminalClient.Domain.Overview;

// The Overview sections in fixed top-to-bottom visual order. No Date is always present and full-width;
// Overdue appears only when it holds at least one visible day; Current always shows its 14 Monday-based
// cells; Future shows only the dates that carry a task.
public enum OverviewSection
{
    NoDate,
    Overdue,
    Current,
    Future,
}
