using AutoMapper;
using DarkDeeds.Services.Mapping;

namespace DarkDeeds.Tests
{
    public class BaseTest
    {
        static BaseTest()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}