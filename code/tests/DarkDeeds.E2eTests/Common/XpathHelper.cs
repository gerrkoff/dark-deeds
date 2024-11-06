namespace DarkDeeds.E2eTests.Common;

public static class XpathHelper
{
    public static string ClassContains(string className)
    {
        return $"[contains(concat(' ', @class, ' '), ' {className} ')]";
    }

    public static string TextContains(string text)
    {
        return $"[text()='{text}']";
    }

    public static string NotContainsAttr(string attr)
    {
        return $"[not(@{attr})]";
    }

    public static string TaskWithText(string text)
    {
        return $"span{ClassContains("task-item")}{TextContains(text)}";
    }

    public static string CreateRecurrenceForm()
    {
        return $"form{ClassContains("recurrences-view-recurrence-item-form")}";
    }

    public static string CreateRecurrenceFormWeekdays()
    {
        return "*[@data-test-id='create-recurrence-form-weekdays']";
    }

    public static string RecurrenceList()
    {
        return $"*{ClassContains("recurrences-view-recurrence-list")}";
    }

    public static string RecurrenceItem()
    {
        return $"*{ClassContains("recurrences-view-recurrence-item")}";
    }

    public static string RecurrenceItemButton(string position)
    {
        return $"*{ClassContains("recurrences-view-recurrence-item-btn")}{ClassContains(position)}";
    }

    public static string Toast(string text)
    {
        var container = $"*{ClassContains("Toastify__toast-container")}";
        var body = $"*{ClassContains("Toastify__toast-body")}";
        var xpath = $"//{container}//{body}";
        if (!string.IsNullOrWhiteSpace(text))
        {
            xpath += $"{TextContains(text)}";
        }

        return xpath;
    }
}
