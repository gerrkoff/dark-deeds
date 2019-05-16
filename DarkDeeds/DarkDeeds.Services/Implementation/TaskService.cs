using System;
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
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskEntity> _tasksRepository;
        
        public TaskService(IRepository<TaskEntity> tasksRepository)
        {
            _tasksRepository = tasksRepository;
        }
        
        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            IQueryable<TaskEntity> tasks = _tasksRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x =>
                    !x.IsCompleted ||
                    !x.DateTime.HasValue ||
                    x.DateTime >= from);
            
            return (await tasks.ProjectTo<TaskDto>().ToListSafeAsync()).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            IQueryable<TaskEntity> tasks = _tasksRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x => x.DateTime.HasValue)
                .Where(x => x.DateTime >= from && x.DateTime < to);

            return (await tasks.ProjectTo<TaskDto>().ToListSafeAsync()).ToUtcDate();
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            await CheckIfUserCanEditTasks(tasks, userId);
            
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
                    task.UserId = userId;
                    await _tasksRepository.SaveAsync(task);
                }
            }

            return Mapper.Map<List<TaskDto>>(savedTasks);
        }

        public async Task CheckIfUserCanEditTasks(ICollection<TaskDto> tasks, string userId)
        {
            int[] taskIds = tasks.Select(x => x.Id).ToArray();
            
            bool notUserTasks = await _tasksRepository.GetAll().AnySafeAsync(x =>
                !string.Equals(x.UserId, userId) &&
                taskIds.Contains(x.Id)); 
            
            if (notUserTasks)
                throw ServiceException.InvalidEntity("Task");
        }
    }
}