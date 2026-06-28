namespace DD.ServiceTask.Domain.Exceptions;

[Serializable]
public class ServiceException : Exception
{
    public ServiceException()
    {
    }

    public ServiceException(string message)
        : base(message)
    {
    }

    public ServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static ServiceException InvalidEntity(string name)
    {
        return new ServiceException($"{name} is invalid for this operation");
    }

    public static ServiceException InvalidDateRange(int maxDays)
    {
        return new ServiceException($"Date range is invalid or exceeds the maximum of {maxDays} days");
    }
}
