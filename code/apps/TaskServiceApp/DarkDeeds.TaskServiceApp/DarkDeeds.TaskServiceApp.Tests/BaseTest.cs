using AutoMapper;
using DarkDeeds.TaskServiceApp.Models.Mapping;

namespace DarkDeeds.TaskServiceApp.Tests
{
    public class BaseTest
    {
        protected static readonly IMapper Mapper;
        static BaseTest()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<ModelsMappingProfile>();
            });
            Mapper = config.CreateMapper();
        }
    }
}