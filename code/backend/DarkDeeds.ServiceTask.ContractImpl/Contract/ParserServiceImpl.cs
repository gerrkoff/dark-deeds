using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.ServiceTask.Contract;
using DarkDeeds.ServiceTask.Dto;
using DarkDeeds.ServiceTask.Services.Interface;
using Grpc.Core;

namespace DarkDeeds.ServiceTask.ContractImpl.Contract
{
    public class ParserServiceImpl : ParserService.ParserServiceBase
    {
        private readonly IMapper _mapper;
        private readonly ITaskParserService _taskParserService;

        public ParserServiceImpl(IMapper mapper, ITaskParserService taskParserService)
        {
            _mapper = mapper;
            _taskParserService = taskParserService;
        }

        public override Task<ParseReply> Parse(ParseRequest request, ServerCallContext context)
        {
            var result = _taskParserService.ParseTask(request.Text);
            return Task.FromResult(new ParseReply
            {
                Task = _mapper.Map<TaskModel>(result)
            });
        }

        public override Task<PrintReply> Print(PrintRequest request, ServerCallContext context)
        {
            var tasks = _mapper.Map<IEnumerable<TaskDto>>(request.Tasks);
            var result = _taskParserService.PrintTasks(tasks);
            return Task.FromResult(new PrintReply
            {
                Texts = {result}
            });
        }
    }
}
