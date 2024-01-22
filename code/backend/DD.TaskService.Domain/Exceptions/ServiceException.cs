namespace DD.TaskService.Domain.Exceptions;

[Serializable]
public class ServiceException(string message) : Exception(message)
{
    public static ServiceException InvalidEntity(string name)
    {
        return new ServiceException($"{name} is invalid for this operation");
    }
}
