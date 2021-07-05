namespace DarkDeeds.E2eTests.Common
{
    public static class Xpath
    {
        public static string ClassContains(string className) => $"[contains(concat(' ', @class, ' '), ' {className} ')]";
        public static string TextContains(string text) => $"[text()='{text}']";
        public static string NotContainsAttr(string attr) => $"[not(@{attr})]";
        
        public static string TaskWithText(string text) => $"span{ClassContains("task-item")}{TextContains(text)}";

        public static string CreateRecurrenceForm() => $"form{ClassContains("recurrences-view-recurrence-item-form")}";
            
        public static string CreateRecurrenceFormWeekdays() => "*[@data-test-id='create-recurrence-form-weekdays']";

        public static string SavingIndicator() => "*[@data-test-id='saving-indicator']";
        
        public static string RecurrenceList() => $"*{ClassContains("recurrences-view-recurrence-list")}";
        
        public static string RecurrenceItem() => $"*{ClassContains("recurrences-view-recurrence-item")}";

        public static string RecurrenceItemButton(string position) =>
            $"*{ClassContains("recurrences-view-recurrence-item-btn")}{ClassContains(position)}";

        public static string Toast(string text)
        {
            var success = $"*{ClassContains("Toastify__toast--success")}";
            
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
}