using AutoMapper;
using DD.ServiceTask.Domain.Mapping;

namespace DD.ServiceTask.Tests.Unit;

public class BaseTest
{
    protected static readonly IMapper Mapper;
    static BaseTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ModelsMappingProfile>();
        });
        Mapper = config.CreateMapper();
    }
}
