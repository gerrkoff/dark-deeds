using AutoMapper;
using DarkDeeds.ServiceTask.Models.Mapping;

namespace DarkDeeds.ServiceTask.Tests
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