namespace DD.Shared.TaskText;

[Serializable]
public class TaskTextParseException : Exception
{
    public TaskTextParseException()
    {
    }

    public TaskTextParseException(string message)
        : base(message)
    {
    }

    public TaskTextParseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
