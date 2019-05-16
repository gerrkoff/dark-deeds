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

            int[] taskIds = tasks.Select(x => x.Id).Where(x => x > 0).ToArray();
            
            Dictionary<int, TaskEntity> existingTasks = await _tasksRepository.GetAll()
                .Where(x => taskIds.Contains(x.Id))
                .ToDictionarySafeAsync(x => x.Id, x => x);

            var savedTasks = new List<TaskDto>();
            foreach (var task in tasks)
            {
                TaskDto savedTask = await SaveTaskAsync(existingTasks, task, userId);
                if (savedTask != null)
                    savedTasks.Add(savedTask);
            }

            return savedTasks;
        }

        private async Task<TaskDto> SaveTaskAsync(Dictionary<int, TaskEntity> existingTasks, TaskDto taskToSave, string userId)
        {   
            if (taskToSave.Deleted || taskToSave.ClientId >= 0)
            {
                if (!existingTasks.ContainsKey(taskToSave.Id))
                {
                    // TODO: log it
                    return null;                    
                }

                if (existingTasks[taskToSave.Id].Version != taskToSave.Version)
                    return ConvertTaskToDto(existingTasks[taskToSave.Id], taskToSave.ClientId);
            }

            // delete
            if (taskToSave.Deleted)
            {
                await _tasksRepository.DeleteAsync(taskToSave.Id);
                return taskToSave;
            }

            TaskEntity entity;
            // create
            if (taskToSave.ClientId < 0)
            {
                entity = Mapper.Map<TaskEntity>(taskToSave);
                entity.Id = 0;
                entity.UserId = userId;
                await _tasksRepository.SaveAsync(entity);
            }
            // update
            else
            {
                entity = existingTasks[taskToSave.Id];
                // TODO: refactor
                entity.DateTime = taskToSave.DateTime;
                entity.Title = taskToSave.Title;
                entity.Order = taskToSave.Order;
                entity.IsCompleted = taskToSave.Completed;
                entity.IsProbable = taskToSave.IsProbable;
                entity.TimeType = taskToSave.TimeType;
                entity.Version++;
                await _tasksRepository.SaveAsync(entity);
            }

            return ConvertTaskToDto(entity, taskToSave.ClientId);
        }

        private TaskDto ConvertTaskToDto(TaskEntity entity, int clientId)
        {
            var dto = Mapper.Map<TaskDto>(entity);
            dto.ClientId = clientId;
            return dto;
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