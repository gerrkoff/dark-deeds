﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskHubService
    {
        Task Update(IEnumerable<TaskDto> updatedTasks);
    }
}