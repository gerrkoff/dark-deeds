using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Common.Exceptions;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models;
using DarkDeeds.Services.Entity;
using DarkDeeds.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskEntity> _tasksRepository;
        
        public TaskService(IRepository<TaskEntity> tasksRepository)
        {
            _tasksRepository = tasksRepository;
        }
        
        public async Task<IEnumerable<TaskDto>> LoadTasksAsync(CurrentUser user)
        {
            return (
                await _tasksRepository
                    .GetAll()
                    .Where(x => string.Equals(x.UserId, user.UserId))
                    .ProjectTo<TaskDto>()
                    .ToListSafeAsync()
            ).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, CurrentUser user)
        {
            await CheckIfUserCanEditTasks(tasks, user);
            
            var savedTasks = Mapper.Map<ICollection<TaskEntity>>(tasks);
            foreach (var task in savedTasks)
            {
                if (task.IsDeleted)
                {
                    await _tasksRepository.DeleteAsync(task);
                }
                else
                {
                    if (task.ClientId < 0)
                    {
                        task.UserId = user.UserId;
                        task.Id = 0;
                    }
                    
                    await _tasksRepository.SaveAsync(task);
                }
            }

            return Mapper.Map<List<TaskDto>>(savedTasks);
        }

        public async Task CheckIfUserCanEditTasks(ICollection<TaskDto> tasks, CurrentUser user)
        {
            int[] taskIds = tasks.Select(x => x.Id).ToArray();
            
            bool notUserTasks = await _tasksRepository.GetAll().AnySafeAsync(x =>
                !string.Equals(x.UserId, user.UserId) &&
                taskIds.Contains(x.Id)); 
            
            if (notUserTasks)
                throw ServiceException.InvalidEntity("Task");
        }
    }
}