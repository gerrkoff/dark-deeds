using System;

namespace DarkDeeds.ServiceTask.Services.Interface
{
    public interface IDateService
    {
        DateTime Today { get; }
        
        DateTime Now { get; }
    }
}