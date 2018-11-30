using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models;
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
        
        private static int Count = 1000000;
        
        public async Task<IEnumerable<TaskDto>> LoadTasksAsync()
        {
            return await _tasksRepository.GetAll().ProjectTo<TaskDto>().ToListAsync();
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks)
        {
            foreach (var taskDto in tasks)
            {
                if (taskDto.ClientId < 0) taskDto.Id = Count++;
            }
            return await Task.FromResult(tasks);
        }
    }
}