using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TaskServiceApp.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly ITaskParserService _taskParserService;

        public ParserController(ITaskParserService taskParserService)
        {
            _taskParserService = taskParserService;
        }


        [HttpGet]
        public TaskDto Get([Required] string text)
        {
            return _taskParserService.ParseTask(text);
        }
        
        [HttpGet(nameof(Print))]
        public IEnumerable<string> Print([FromBody] ICollection<TaskDto> tasks)
        {
            return _taskParserService.PrintTasks(tasks);
        }
    }
}