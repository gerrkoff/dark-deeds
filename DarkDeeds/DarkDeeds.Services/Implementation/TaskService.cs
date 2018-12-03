using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Common.Extensions;
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
        
        public async Task<IEnumerable<TaskDto>> LoadTasksAsync()
        {
            return (await _tasksRepository.GetAll().ProjectTo<TaskDto>().ToListAsync()).ToUtcDate();
        }

        // TODO: unit test
        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks)
        {
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
                    await _tasksRepository.SaveAsync(task);
                }
            }

            return Mapper.Map<List<TaskDto>>(savedTasks);
        }
    }
}