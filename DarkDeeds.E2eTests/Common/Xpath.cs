namespace DarkDeeds.E2eTests.Common
{
    public static class Xpath
    {
        public static string ClassContains(string className) => $"[contains(concat(' ', @class, ' '), ' {className} ')]";
        public static string TextContains(string text) => $"[text()='{text}']";
        
        public static string TaskWithText(string text) => $"span{ClassContains("task-item")}{TextContains(text)}";
    }
}