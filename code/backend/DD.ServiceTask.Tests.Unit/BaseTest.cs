using AutoMapper;
using DD.ServiceTask.Domain.Mapping;

namespace DD.ServiceTask.Tests.Unit;

public class BaseTest
{
    protected static readonly IMapper Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ModelsMappingProfile>();
        })
        .CreateMapper();
}
