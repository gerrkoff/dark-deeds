using System;

namespace DarkDeeds.ServiceTask.Models.Exceptions
{
    [Serializable]
    public class ServiceException : Exception
    {   
        public ServiceException(string message) : base(message)
        {
        }
        
        public static ServiceException InvalidEntity(string name)
        {
            return new ServiceException($"{name} is invalid for this operation");
        }
    }
}