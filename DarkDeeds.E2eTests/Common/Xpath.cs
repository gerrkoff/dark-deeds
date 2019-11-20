namespace DarkDeeds.E2eTests.Common
{
    public static class Xpath
    {
        public static string ClassContains(string className) => $"[contains(concat(' ', @class, ' '), ' {className} ')]";
        public static string TextContains(string text) => $"[text()='{text}']";
        
        public static string TaskWithText(string text) => $"span{ClassContains("task-item")}{TextContains(text)}";

        public static string CreateRecurrenceForm() => $"form{ClassContains("recurrences-view-recurrence-item-form")}";
            
        public static string CreateRecurrenceFormWeekdays() => $"*[@data-test-id='create-recurrence-form-weekdays']";

        public static string SavingIndicator() => "*[@data-test-id='savingIndicator']";
    }
}