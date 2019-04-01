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
        
        public async Task<IEnumerable<TaskDto>> LoadTasksAsync(string userId,
            DateTime? from,
            DateTime? to,
            bool includeNoDate)
        {
            if (from == null)
            {
                TaskEntity firstUncompletedTask = await _tasksRepository
                    .GetAll()
                    .Where(x => string.Equals(x.UserId, userId))
                    .Where(x => x.DateTime.HasValue)
                    .OrderBy(x => x.DateTime.Value)
                    .FirstOrDefaultSafeAsync(x => !x.IsCompleted);
                
                var currentPeriodStart = DateTime.UtcNow.AddDays(-1);

                from = firstUncompletedTask != null && firstUncompletedTask.DateTime < currentPeriodStart
                    ? firstUncompletedTask.DateTime
                    : currentPeriodStart;
            }

            IQueryable<TaskEntity> tasks = _tasksRepository
                .GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .Where(x =>
                    !x.DateTime.HasValue && includeNoDate ||
                    x.DateTime.HasValue && x.DateTime >= from.Value);

            if (to != null)
                tasks = tasks.Where(x => 
                        !x.DateTime.HasValue && includeNoDate ||
                        x.DateTime.HasValue && x.DateTime < to.Value);

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