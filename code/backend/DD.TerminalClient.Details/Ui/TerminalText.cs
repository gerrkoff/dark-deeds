using System.Globalization;
using System.Text;
using Spectre.Console;

namespace DD.TerminalClient.Details.Ui;

// Shared plain-text helpers for the renderer: control-character sanitizing, cell-width-aware single-line
// truncation and time formatting. Truncation measures East-Asian-wide and multi-code-unit runes with
// Spectre's cell metrics so a truncated line never exceeds the target width and never splits a rune,
// which is what keeps every task on exactly one line and the line metadata deterministic.
internal static class TerminalText
{
    private const char Ellipsis = '\u2026';

    // Strips control characters (including newlines and tabs) so a task title can never spill onto a
    // second line or emit stray escape sequences.
    public static string Sanitize(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var rune in text.EnumerateRunes())
        {
            if (!Rune.IsControl(rune))
            {
                builder.Append(rune.ToString());
            }
        }

        return builder.ToString();
    }

    // Returns the sanitized text if it fits maxWidth cells, otherwise the longest rune prefix that fits
    // together with a trailing ellipsis. A non-positive width yields an empty string.
    public static string Truncate(string text, int maxWidth)
    {
        if (maxWidth <= 0)
        {
            return string.Empty;
        }

        var sanitized = Sanitize(text);
        if (sanitized.GetCellWidth() <= maxWidth)
        {
            return sanitized;
        }

        if (maxWidth == 1)
        {
            return Ellipsis.ToString();
        }

        var budget = maxWidth - 1;
        var used = 0;
        var builder = new StringBuilder();
        foreach (var rune in sanitized.EnumerateRunes())
        {
            var runeText = rune.ToString();
            var runeWidth = runeText.GetCellWidth();
            if (used + runeWidth > budget)
            {
                break;
            }

            used += runeWidth;
            builder.Append(runeText);
        }

        builder.Append(Ellipsis);
        return builder.ToString();
    }

    // Formats minutes from midnight as a 24-hour HH:mm label, matching the web dateService.toTimeLabel.
    public static string FormatTime(int minutes)
    {
        var hours = minutes / 60;
        var mins = minutes % 60;
        return hours.ToString("D2", CultureInfo.InvariantCulture)
            + ":"
            + mins.ToString("D2", CultureInfo.InvariantCulture);
    }
}
