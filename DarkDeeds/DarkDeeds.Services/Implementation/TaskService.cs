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
using Microsoft.EntityFrameworkCore.Internal;

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
                    .Where(x => string.Equals(x.User.Id, user.UserId))
                    .ProjectTo<TaskDto>()
                    .ToListAsync()
            ).ToUtcDate();
        }

        // TODO: unit test
        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, CurrentUser user)
        {
            CheckIfUserCanEditTasks(tasks, user);
            
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
                        task.Id = 0;
                    task.UserId = user.UserId;
                    await _tasksRepository.SaveAsync(task);
                }
            }

            return Mapper.Map<List<TaskDto>>(savedTasks);
        }

        // TODO: unit test
        public void CheckIfUserCanEditTasks(ICollection<TaskDto> tasks, CurrentUser user)
        {
            int[] taskIds = tasks.Select(x => x.Id).ToArray();
            bool notUserTasks = _tasksRepository.GetAll().Any(x =>
                !string.Equals(x.UserId, user.UserId) &&
                taskIds.Contains(x.Id)); 
            
            if (notUserTasks)
                throw ServiceException.InvalidEntity("Task");
        }
    }
}