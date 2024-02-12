namespace DD.TelegramClient.Domain.Exceptions;

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
}
