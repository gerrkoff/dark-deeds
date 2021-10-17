using System;

namespace DarkDeeds.TaskServiceApp.Services.Interface
{
    public interface IDateService
    {
        DateTime Today { get; }
        
        DateTime Now { get; }
    }
}